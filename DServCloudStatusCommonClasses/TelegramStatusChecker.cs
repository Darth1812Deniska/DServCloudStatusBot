using System;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DServCloudStatusCommonClasses
{
    public class TelegramStatusChecker
    {
        private readonly TelegramBotClient _telegramBotClient;
        private Logger Logger => LogManager.GetCurrentClassLogger();

        public TelegramStatusChecker(string botToken)
        {
            _telegramBotClient = new TelegramBotClient(botToken);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // разрешено получать все виды апдейтов
            };
            _telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
            Logger.Info("Запущено");
        }

        private TelegramBotClient TelegramBotClient => _telegramBotClient;

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
            Chat chat;
            if (update.Message != null)
            {
                chat = update.Message.Chat;
                Logger.Info($"{updateType} - {chat.Id}");
            }
            else
            {
                return;
            }

            if (updateType == UpdateType.Message)
            {
                var message = update.Message;
                if (message != null && message.Text != null)
                {
                    string messageText = message.Text;
                    Logger.Info($"{updateType} - {messageText}");
                    switch (messageText)
                    {
                        case @"/start":
                            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"Добро пожаловать! Бот запущен");
                            break;
                        case @"/server_restart":
                            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"Команда на перезапуск обработана");
                            await ServerRestartAsync(message);
                            break;
                        case @"/router_restart":
                            await TelegramBotClient.SendTextMessageAsync(chat.Id,
                                $"Команда на перезапуск роутера обработана");
                            await RouterRestartAsync(message);
                            break;
                        case @"/status":
                            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"Сервис работает");
                            await SendServerStatusAsync(message);
                            break;
                        case @"/ip_status":
                            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"Команда на пинг IP обработана");
                            await SendIpStatusAsync(message);
                            break;
                        default:
                            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"Команда не поддерживается");
                            break;
                    }
                }
            }
        }

        private async Task ServerRestartAsync(Message message)
        {
            User? fromUser = message.From;
            Chat chat = message.Chat;
            if (fromUser==null)
                return;
            long userId = fromUser.Id;
            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"{userId}");
        }

        private async Task RouterRestartAsync(Message message)
        {
            User? fromUser = message.From;
            Chat chat = message.Chat;
            if (fromUser == null)
                return;
            long userId = fromUser.Id;
            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"{userId}");
        }

        private async Task SendServerStatusAsync(Message message)
        {
            User? fromUser = message.From;
            Chat chat = message.Chat;
            if (fromUser == null)
                return;
            long userId = fromUser.Id;
            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"{userId}");
        }

        private async Task SendIpStatusAsync(Message message)
        {
            User? fromUser = message.From;
            Chat chat = message.Chat;
            if (fromUser == null)
                return;
            long userId = fromUser.Id;
            await TelegramBotClient.SendTextMessageAsync(chat.Id, $"{userId}");
        }
    }
}