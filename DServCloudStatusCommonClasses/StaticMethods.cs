using NLog;
using System.Net.Sockets;

namespace DServCloudStatusCommonClasses
{
    public static class StaticMethods
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public static bool PingHost(string hostUri, int portNumber)
        {
            try
            {
                using (var client = new TcpClient(hostUri, portNumber))
                {
                    Logger.Info($"PingHost. \"{hostUri}:{portNumber}\" - доступно");
                    return true;
                }
            }
            catch (SocketException ex)
            {
                Logger.Error($"PingHost. {ex.Message}");
                return false;
            }
        }
    }
}
