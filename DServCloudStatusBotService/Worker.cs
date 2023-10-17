using System.Timers;
using DServCloudStatusCommonClasses;
using NLog;
using Timer = System.Timers.Timer;

namespace DServCloudStatusBotService
{
    public class Worker : BackgroundService
    {
        private Logger Logger => LogManager.GetCurrentClassLogger();
        private BotSettings BotSettings => _botSettings;
        private readonly BotSettings _botSettings;
        private TelegramStatusChecker _telegramStatusChecker;
        private TelegramStatusChecker TelegramStatusChecker => _telegramStatusChecker;

        public Worker(ILogger<Worker> logger, BotSettings botSettings)
        {
            this._botSettings = botSettings;
            _telegramStatusChecker = new TelegramStatusChecker(BotSettings);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Timer timer = new Timer(2000);
            timer.Elapsed += ExecuteTimer_Elapsed;
        }

        private void ExecuteTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.Info($"StartAsync. Запуск сервиса");
            
            Timer startTimer = new Timer(3000);
            startTimer.Elapsed += StartTimerOnElapsed;
            startTimer.Start();
        }

        private void StartTimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            Logger.Info($"StartTimerOnElapsed. Итерация таймера {e.SignalTime}");
            bool isPing = StaticMethods.PingHost("api.telegram.org", 443);
            if (isPing) 
            {
                try
                {
                    TelegramStatusChecker.SendMessageToAdminAsync("Сервис запущен");
                }
                catch (Exception exception)
                {
                    Logger.Error($"StartAsync. {exception.Message}; {exception}");
                }

                var timer = sender as Timer;
                timer.Stop();
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await TelegramStatusChecker.SendMessageToAdminAsync("Сервис останавливается");
            }
            catch (Exception e)
            {
                Logger.Error($"StopAsync. {e.Message}; {e}");
            }
        }
    }
}