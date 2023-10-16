using System;
using System.Net.NetworkInformation;
using System.Net;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Net.Sockets;
using System.Linq;
using System.Diagnostics;

namespace DServCloudStatusCommonClasses
{
    public class TelegramStatusChecker
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly string _adminUserName;
        private readonly string _serverIpAddressString;
        private readonly int[] _portsToCheck;
        private readonly BotSettings _botSettings;
        private TelegramBotClient TelegramBotClient => _telegramBotClient;
        private Logger Logger => LogManager.GetCurrentClassLogger();

        private BotSettings BotSettings => _botSettings;

        private string AdminUserName => _adminUserName;

        private string ServerIpAddressString => _serverIpAddressString;

        private int[] PortsToCheck => _portsToCheck;

        public TelegramStatusChecker(BotSettings botSettings)
        {
            _botSettings = botSettings;
            _adminUserName = BotSettings.AdminUserName;
            Logger.Info($"Задано имя администратора: AdminUserName = \"{AdminUserName}\"");
            _serverIpAddressString = BotSettings.ServerIpAddress;
            Logger.Info($"Задан внешний IP: ServerIpAddressString = \"{ServerIpAddressString}\"");
            _portsToCheck = BotSettings.PortsToCheck;
            string.Join(",", PortsToCheck);
            Logger.Info($"Указаны следующие порты для проверки: PortsToCheck = \"{string.Join(",", PortsToCheck)}\"");
            BotUsers.LoadUsers();
            _telegramBotClient = new TelegramBotClient(BotSettings.BotToken);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // разрешено получать все виды апдейтов
            };
             _telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
            Logger.Info("Бот запущен!");
        }

        private async Task HandleErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            Logger.Error($"HandleErrorAsync. {exception.Message}; {exception}");
        }

        private async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken
        )
        {
            UpdateType updateType = update.Type;
            if (updateType == UpdateType.Message)
            {
                var message = update.Message;
                if (message == null)
                    return;
                Chat chat = message.Chat;
                User? fromUser = message.From;
                if (fromUser == null) 
                    return;
                long userId = fromUser.Id;
                string? username = fromUser.Username;

                if (!BotUsers.IsUserExists(username))
                {
                    if (BotSettings.AdminUserName == username)
                    {
                        BotUsers.AddAdminUser(userId, username, chat.Id);
                    }
                    else
                    {
                        await SendMessageToAdminAsync($"Прислано сообщение от нового пользователя \"{username}\"");
                        BotUsers.AddUser(userId, username, chat.Id, false);
                    }
                }

                Logger.Info(
                    $"Новое сообщение в чате chatID = {chat.Id} от пользователя \"{username}\" (userID = {userId})");
                if (message.Text != null)
                {
                    string messageText = message.Text;
                    Logger.Info($"{updateType} - {messageText}");
                    switch (messageText)
                    {
                        case @"/start":
                            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"Добро пожаловать! Бот запущен",
                                cancellationToken: cancellationToken);
                            break;
                        case @"/server_restart":
                            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"Команда на перезапуск обработана",
                                cancellationToken: cancellationToken);
                            await ServerRestartAsync(message);
                            break;
                        case @"/server_restart_confirm":
                            await ServerRestartConfirmAsync(message);
                            break;
                        case @"/server_restart_cancel":
                            await ServerRestartCancelAsync(message);
                            break;
                        case @"/router_restart":
                            await TelegramBotClient.SendTextMessageAsync(chat.Id,
                                $"Команда на перезапуск роутера обработана", cancellationToken: cancellationToken);
                            await RouterRestartAsync(message);
                            break;
                        case @"/status":
                            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"Сервис работает",
                                cancellationToken: cancellationToken);
                            await SendServerStatusAsync(message);
                            break;
                        case @"/ip_status":
                            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"Команда на пинг IP обработана",
                                cancellationToken: cancellationToken);
                            await SendIpStatusAsync(chat, username);
                            break;
                        default:
                            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"Команда не поддерживается",
                                cancellationToken: cancellationToken);
                            break;
                    }
                }
            }
        }

        private async Task ServerRestartAsync(Message message)
        {
            Logger.Info($"ServerRestartAsync. Запрошен перезапуск сервера");
            var chat = message.Chat;
            if (chat == null)
            {
                Logger.Info($"ServerRestartAsync. Не получен chat. Не перезапускается");
                return;
            }

            var user = message.From;
            if (user == null)
            {
                Logger.Info($"ServerRestartAsync. Не указан пользователь. не перезапускается");
                return;
            }

            string username = user.Username;
            if (username != AdminUserName && !BotUsers.IsUserAdmin(username))
            {
                Logger.Info($"ServerRestartAsync. Перезапуск запрошен не администратором - не перезапускается");
                return;
            }

            Logger.Info($"ServerRestartAsync. Перезапуск запрошен администратором - перезапускается");
            await TelegramBotClient.SendTextMessageAsync(chat.Id,
                @"Запрошена перезагрузка сервера. 
Для перезапуска нажми: /server_restart_confirm, для отмены: /server_restart_cancel");
            
        }

        private async Task ServerRestartConfirmAsync(Message message)
        {
            Chat chat = message.Chat;

            Logger.Info($"ServerRestartConfirm. Перезапуск подтвержден администратором");
            await TelegramBotClient.SendTextMessageAsync(chat.Id, "Перезапуск подтвержден администратором");
            Restart();
        }

        private void Restart()
        {
            Logger.Info($"Restart. Перезапуск через 30 секунд");
            StartShutDown("-f -r -t 30");
        }

        private static void StartShutDown(string param)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.FileName = "cmd";
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Arguments = "/C shutdown " + param;
            Process.Start(proc);
        }

        private async Task ServerRestartCancelAsync(Message message)
        {
            Chat chat = message.Chat;
            Logger.Info($"ServerRestartConfirm. Перезапуск отменен администратором");
            await TelegramBotClient.SendTextMessageAsync(chat.Id, "Перезапуск отменен администратором");
        }

        private async Task RouterRestartAsync(Message message)
        {
        }

        private async Task SendServerStatusAsync(Message message)
        {
        }

        public async Task SendMessageToAdminAsync(string messageText)
        {
            if (string.IsNullOrEmpty(messageText.Trim()))
            {
                Logger.Error("SendMessageToAdminAsync. Попытка отправки пустого сообщения");
                return;
            }

            if (BotUsers.AdminUsersList.Count == 0)
            {
                Logger.Error("SendMessageToAdminAsync. Не указаны администраторы для получения сообщения");
                return;
            }

            foreach (var adminUser in BotUsers.AdminUsersList)
            {
                var chatID = adminUser.ChatId;
                var adminName =adminUser.Name;
                await TelegramBotClient.SendTextMessageAsync(chatID, messageText);
                Logger.Info($"SendMessageToAdminAsync. Отправлено админу \"{adminName}\" в чат {chatID} сообщение: \"{messageText}\"");
            }
        }

        private async Task SendIpStatusAsync(Chat chat, string? username)
        {
            Logger.Info($"SendIpStatusAsync. Запрошена проверка портов");
            if (username != AdminUserName && BotUsers.IsUserAdmin(username))
            {
                Logger.Info($"SendIpStatusAsync. Проверку запросил не админ. Отмена");
                return;
            }

            Logger.Info($"SendIpStatusAsync. Проверку запросил админ. Выполняется");

            foreach (var port in PortsToCheck)
            {
                Logger.Info($"SendIpStatusAsync. Выполняется проверка {ServerIpAddressString}:{port}");
                bool isPing = PingHost(ServerIpAddressString, port);
                if (isPing)
                {
                    Logger.Info($"SendIpStatusAsync. {ServerIpAddressString}:{port} - доступен");
                    await TelegramBotClient.SendTextMessageAsync(chat.Id, $"{ServerIpAddressString}:{port} - доступен");
                }
                else
                {
                    Logger.Info($"SendIpStatusAsync. {ServerIpAddressString}:{port} - недоступен");
                    await TelegramBotClient.SendTextMessageAsync(chat.Id,
                        $"{ServerIpAddressString}:{port} - недоступен");
                }
            }
        }

        private bool PingHost(string hostUri, int portNumber)
        {
            try
            {
                using (var client = new TcpClient(hostUri, portNumber))
                    return true;
            }
            catch (SocketException ex)
            {
                Logger.Error($"PingHost. {ex.Message}");
                return false;
            }
        }
    }
}