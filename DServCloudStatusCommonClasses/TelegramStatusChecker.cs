using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DServCloudStatusCommonClasses
{
    public class TelegramStatusChecker
    {
        private readonly TelegramBotClient _telegramBotClient;

        public TelegramStatusChecker(string botToken)
        {
            _telegramBotClient = new TelegramBotClient(botToken);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates ={} , // разрешено получать все виды апдейтов
            };
            _telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
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
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));

            // Тут бот получает сообщения от пользователя
            // Дальше код отвечает за команду старт, которую можно добавить через botfather
            // Если все хорошо при запуске program.cs в консоль выведется, что бот запущен
            // а при отправке команды бот напишет Привет

            var message = update.Message;
            if (message != null && message.Text != null)
            {
                string messageText = message.Text;
                Console.WriteLine(messageText);
            }


            if (update.Type == UpdateType.CallbackQuery)
            {
                // Тут получает нажатия на inline кнопки
            }
        }
    }
}