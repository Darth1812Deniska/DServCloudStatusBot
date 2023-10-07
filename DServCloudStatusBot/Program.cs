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

//TelegramStatusChecker statusChecker = new TelegramStatusChecker(botSettings);

Dictionary<string, string> contentDictionary = new Dictionary<string, string>()
{
    {"action", "login"},
    //{"Password", "40689c2a8b1b10f1b6af0a582db3bf2bd0a147712d8b7fdd79050435d05c5fea"},
    {"Password", "mtsoao"},
    {"Username", "mgts"}

};
FormUrlEncodedContent content = new FormUrlEncodedContent(contentDictionary);
UriBuilder builder = new UriBuilder();
builder.Host = "192.168.1.1";
builder.Query = "_type=loginData&_tag=login_entry";
Uri clientUri = builder.Uri;
Console.WriteLine(builder.Uri);

using (HttpClient client = new HttpClient())
{
    client.BaseAddress = new Uri("http://192.168.1.1/");
    var result = client.PostAsync(clientUri, content).Result;
    Console.WriteLine(result.Content.ToString());
    Console.WriteLine(result.Content.ReadAsStringAsync().Result);
    Console.WriteLine(result.EnsureSuccessStatusCode().ToString());
}


Console.ReadLine();

