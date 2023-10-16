using NLog;
using System.Text.Json;

namespace DServCloudStatusCommonClasses;

public static class BotUsers
{
    private static Logger Logger => LogManager.GetCurrentClassLogger();
    private const string  BotUsersListFileName = "BotUsersList.json";

    private static  List<BotUser> _BotUsersList = new List<BotUser>();
    private static string BotUsersListFullPathFileName => GetBotUsersListFullPathFileName();

    public static List<BotUser> BotUsersList => _BotUsersList;
    public static List<BotUser> AdminUsersList => GetAdminUsersList();


    private static List<BotUser> GetAdminUsersList()
    {
        return BotUsersList.FindAll(user => user.IsAdmin);
    }

    private static string GetBotUsersListFullPathFileName()
    {
        return $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}{BotUsersListFileName}";
    }

    public static void SaveUsers()
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_BotUsersList);
            File.WriteAllText(BotUsersListFullPathFileName, jsonString);
            Logger.Info($"SaveUsers. Пользователи сохранены");
        }
        catch (Exception e)
        {
            Logger.Error($"SaveUsers. {e};");
        }
        
    }

    public static void LoadUsers()
    {
        try
        {
            if (!File.Exists(BotUsersListFullPathFileName))
            {
                return;
            }
            FileInfo fileInfo = new FileInfo(BotUsersListFullPathFileName);
            Logger.Info($"LoadUsers. расположение файла: {fileInfo.FullName}");

            string jsonString = File.ReadAllText(BotUsersListFullPathFileName);
            _BotUsersList = JsonSerializer.Deserialize<List<BotUser>>(jsonString)
                            ?? new List<BotUser>();
            Logger.Info($"LoadUsers. Пользователи загружены");
        }
        catch (Exception e)
        {
            Logger.Error($"LoadUsers. {e};");
        }
        
    }

    public static void AddUser(BotUser botUser)
    {
        BotUsersList.Add(botUser);
        Logger.Info($"AddUser. Добавлен пользователь \"{botUser}\"");
        SaveUsers();
    }

    public static void AddUser(long id, string name, long chatId, bool isAdmin)
    {
        Logger.Info($"AddUser. Добавление пользователя с параметрами: id={id}; name=\"{name}\"; chatId={chatId}; isAdmin={isAdmin}");
        BotUser botUser = new BotUser()
        {
            Id = id,
            Name = name,
            ChatId = chatId,
            IsAdmin = isAdmin
        };
        AddUser(botUser);
    }

    public static void AddAdminUser(long id, string name, long chatId)
    {
        AddUser(id, name, chatId, true);
        Logger.Info($"AddAdminUser. Добавлен администратор \"{name}\"");
    }

    public static bool IsUserAdmin(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Logger.Error($"IsUserAdmin. На проверку пришла пустая строка");
            return false;
        }

        Logger.Info($"IsUserAdmin. Проверка пользователя \"{name}\"");
        BotUser botUser = BotUsersList.FirstOrDefault(user => user.Name == name);
        if (botUser == null)
            return false;
        return botUser.IsAdmin;
    }

    public static bool IsUserExists(string name)
    {
        Logger.Info($"IsUserExists. Проверка пользователя \"{name}\"");
        return BotUsersList.Exists(user => user.Name == name);
    }
}