namespace DServCloudStatusCommonClasses
{
    public class BotSettings
    {
        private string _botToken;
        private string _adminUserName;
        private string _serverIpAddress;
        private int[] _portsToCheck;

        public string BotToken
        {
            get => _botToken;
            set => _botToken = value;
        }

        public string AdminUserName
        {
            get => _adminUserName;
            set => _adminUserName = value;
        }

        public string ServerIpAddress
        {
            get => _serverIpAddress;
            set => _serverIpAddress = value;
        }

        public int[] PortsToCheck
        {
            get => _portsToCheck;
            set => _portsToCheck = value;
        }

        public BotSettings(string botToken, string adminUserName, string serverIpAddress, int[] portsToCheck)
        {
            _adminUserName = adminUserName;
            _serverIpAddress = serverIpAddress;
            _portsToCheck = portsToCheck;
            _botToken = botToken;
            
        }

        public BotSettings()
        {

        }
    }
}
