using DServCloudStatusCommonClasses;
using Microsoft.Extensions.Configuration;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

IConfigurationSection settings = configuration.GetSection("settings");
string botToken = settings["bot_token"] ?? string.Empty;
string adminUserName = settings["admin_user_name"] ?? string.Empty;
string serverIpAddress = settings["server_ip_address"] ?? string.Empty;
var portToCheck = settings.GetSection("ports_to_check").Get<int[]>();

if (string.IsNullOrEmpty(botToken))
{
    return;
}

TelegramStatusChecker statusChecker = new TelegramStatusChecker(botToken, adminUserName, serverIpAddress, portToCheck);

Console.ReadLine();

