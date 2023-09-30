using DServCloudStatusCommonClasses;
using Microsoft.Extensions.Configuration;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();
if (configuration==null)
    return;
BotSettings botSettings = configuration.GetSection("BotSettings").Get<BotSettings>();
if (botSettings == null)
{
    return;
}

if (string.IsNullOrEmpty(botSettings.BotToken))
{
    return;
}

TelegramStatusChecker statusChecker = new TelegramStatusChecker(botSettings);

Console.ReadLine();

