using DServCloudStatusBotService;
using DServCloudStatusCommonClasses;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        BotSettings botSettings = configuration.GetSection("BotSettings").Get<BotSettings>();
        services.AddSingleton(botSettings);
        services.AddHostedService<Worker>();
    }).UseWindowsService()
    .Build();

host.Run();