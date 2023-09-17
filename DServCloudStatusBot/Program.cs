using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
    .Build();

var settings = configuration.GetSection("settings");
string botToken = settings["bot_token"] ?? string.Empty;
if (string.IsNullOrEmpty(botToken))
{
    return;
}
Console.WriteLine(botToken);
