using System.Net;
using System.Text.Json;
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
statusChecker.SendMessageToAdminAsync("Сервис запущен и это тест");
Console.ReadLine();

//

/*
 {"action", "login"},
        {"Password", "mtsoao"},
        {"Username", "mgts"}
 */

/*
string authorizationJson;

Uri routerUri = new Uri("http://192.168.1.1/");
CookieContainer cookies = new CookieContainer();
var handler = new HttpClientHandler();
handler.CookieContainer = cookies;

using (HttpClient client = new HttpClient(handler))
{
    client.DefaultRequestHeaders.Connection.Add("keep-alive");
    // Получать куки
    // установить куки
    // Получать перед авторизацией токены
    //
    // Получить токены перед перезапуском


    UriBuilder authUriBuilder = new UriBuilder
    {
        Host = "192.168.1.1",
        Query = "_type=loginData&_tag=login_token"
    };
    Uri authUri = authUriBuilder.Uri;
    var authResult = client.GetAsync(authUri).Result.Content.ReadAsStringAsync().Result;
    Console.WriteLine(authResult.ToString());//<ajax_response_xml_root>78068737</ajax_response_xml_root>

    //   var authHeaders = authResult.Headers;
    //  var authCookies = authHeaders.GetValues("Set-Cookie");
    // string authCookie = authCookies.FirstOrDefault();
    /*string? sid = String.Empty;
    if (authCookie != null)
    {
        sid = authCookie.Split(';').Select(str => str.Trim()).FirstOrDefault(str => str.Contains("SID="));
    }
    Console.WriteLine(sid);
    var cookieSid = sid.Split('=');
    cookies.Add(routerUri, new Cookie(cookieSid[0], cookieSid[1]));
    cookies.Add(routerUri, new Cookie("_TESTCOOKIESUPPORT", "1"));
    
    //Console.WriteLine().ToString());
    //var authResultContent = authResult.Content;
    //string authResultContentString = authResultContent.ReadAsStringAsync().Result;
    //Console.WriteLine(authResultContentString);

    string? sessionToken= GetSessionToken( client, "192.168.1.1", "mgts", "mtsoao");
    if (sessionToken != null)
    {
        Console.WriteLine(sessionToken);
        Dictionary<string, string> contentDictionary = new Dictionary<string, string>()
        {
            { "_sessionTOKEN", sessionToken },
            { "IF_ACTION", "Restart" },
            { "Btn_restart", String.Empty }
        };
        FormUrlEncodedContent content = new FormUrlEncodedContent(contentDictionary);
        UriBuilder builder = new UriBuilder
        {
            Host = "192.168.1.1",
            Query = "_type=menuData&_tag=devmgr_restartmgr_lua.lua"
        };
        Uri clientUri = builder.Uri;
        Console.WriteLine(clientUri.AbsoluteUri);
        var postResult = client.PostAsync(clientUri, content).Result;

        Console.WriteLine(postResult.Content.ReadAsStringAsync().Result);


    }
}


Console.ReadLine();




string? GetSessionToken(HttpClient client, string routerHost, string routerLogin, string routerPassword)
{
    Dictionary<string, string> contentDictionary = new Dictionary<string, string>()
    {
        {"action", "login"},
        {"Password", routerPassword},
        {"Username", routerLogin}

    };
    FormUrlEncodedContent content = new FormUrlEncodedContent(contentDictionary);
    UriBuilder builder = new UriBuilder
    {
        Host = routerHost,
        Query = "_type=loginData&_tag=login_entry"
    };
    Uri clientUri = builder.Uri;
    var postResult = client.PostAsync(clientUri, content).Result;
    authorizationJson = postResult.Content.ReadAsStringAsync().Result;
    JsonDocument json = JsonDocument.Parse(authorizationJson);
    var jsonRoot = json.RootElement;
    return jsonRoot.GetProperty("sess_token").GetString();
}

*/