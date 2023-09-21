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

namespace DServCloudStatusCommonClasses
{
    public class TelegramStatusChecker
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly string _adminUserName;
        private readonly string _serverIpAddressString;
        private readonly int[] _portsToCheck;
        private TelegramBotClient TelegramBotClient => _telegramBotClient;
        private Logger Logger => LogManager.GetCurrentClassLogger();

        private string AdminUserName => _adminUserName;

        private string ServerIpAddressString => _serverIpAddressString;

        private int[] PortsToCheck => _portsToCheck;

        public TelegramStatusChecker(string botToken, string adminUserName, string serverIpAddressString,
            int[] portsToCheck)
        {
            _adminUserName = adminUserName;
            Logger.Info($"Задано имя администратора: AdminUserName = \"{AdminUserName}\"");
            _serverIpAddressString = serverIpAddressString;
            Logger.Info($"Задан внешний IP: ServerIpAddressString = \"{ServerIpAddressString}\"");
            _portsToCheck = portsToCheck;
            string.Join(",", PortsToCheck);
            Logger.Info($"Указаны следующие порты для проверки: PortsToCheck = \"{string.Join(",", PortsToCheck)}\"");
            _telegramBotClient = new TelegramBotClient(botToken);
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
            // Данный Хендлер получает ошибки и выводит их в консоль в виде JSON
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
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
                if (fromUser == null) return;
                long userId = fromUser.Id;
                string? username = fromUser.Username;
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
        }

        private async Task RouterRestartAsync(Message message)
        {
        }

        private async Task SendServerStatusAsync(Message message)
        {
        }

        private async Task SendIpStatusAsync(Chat chat, string? username)
        {
            Logger.Info($"SendIpStatusAsync. Запрошена проверка портов");
            if (username != AdminUserName)
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
                    await TelegramBotClient.SendTextMessageAsync(chat.Id, $"{ServerIpAddressString}:{port} - недоступен");
                }
            }
            
            /*Ping pingSender = new Ping();

            IPAddress serverIpAddress = IPAddress.Parse(ServerIpAddressString);
            PingReply reply = pingSender.Send(serverIpAddress);
            if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine("IP {0} is reachable", reply.Address);
            }
            else
            {
                Console.WriteLine("IP {0} is not reachable, error: {2}", reply.Address, reply.Status);
            }
            */
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