using DServCloudStatusCommonClasses;

namespace DServCloudStatusBotService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private BotSettings BotSettings => _botSettings;
        private readonly BotSettings _botSettings;
        private TelegramStatusChecker _telegramStatusChecker;
        private TelegramStatusChecker TelegramStatusChecker => _telegramStatusChecker;

        public Worker(ILogger<Worker> logger, BotSettings botSettings)
        {
            _logger = logger;
            this._botSettings = botSettings;
            _telegramStatusChecker = new TelegramStatusChecker(BotSettings);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
        }
        
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            
        }
        /*
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            
        }
        
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        */
    }
}