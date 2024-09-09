#define LOCAL_TEST

using ApplicationServer.Manager;
using NetCoreServer;
using Serilog;
using System.Diagnostics;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace ApplicationServer
{
    internal class Program
    {
        private const string Version = "0.1.4";
        private static bool manualShutdown = false;



        static async Task Main(string[] args)
        {
            Log.Information("当前版本号:" + Version);

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            await InitAsync();

            Config config = ConfigManager.GetConfig();

            if (config == null)
            {
                int port = config.Port;
                ConnectionMode mode = config.ConnectionMode; // 从配置中获取连接模式

                switch (mode)
                {
                    case ConnectionMode.Http:
                        Log.Information($"HTTP服务器端口: {IPAddress.Any}:{port}");
                        HttpServer http = new HttpApplicationServer(IPAddress.Any, port);
                        break;
                    case ConnectionMode.Https:
                        Log.Information($"HTTPS服务器端口: {port}");
                        var context = new SslContext(SslProtocols.Tls13, new X509Certificate2("server.pfx", "qwerty"));
                        HttpsServer https = new HttpsApplicationServer(context, IPAddress.Any, port);
                        break;
                    case ConnectionMode.Tcp:
                        break;
                    case ConnectionMode.Udp:
                        break;
                    case ConnectionMode.WebSocket:
                        break;
                    case ConnectionMode.Kcp:
                        break;

                }
            }

            //            if (config != null)
            //            {
            //                int port = config.Port;
            //#if LOCAL_TEST
            //                Log.Information($"HTTP服务器端口: {IPAddress.Any}:{port}");
            //                var server = new HttpApplicationServer(IPAddress.Any, port);
            //#else
            //                Log.Information($"HTTPS服务器端口: {port}");
            //                var context = new SslContext(SslProtocols.Tls13, new X509Certificate2("server.pfx", "qwerty"));
            //                var server = new HttpsApplicationServer(context, IPAddress.Any, port);
            //#endif
            //                Log.Information("服务器启动中。。。");
            //                Thread serverThread = new Thread(() => server.Start());
            //                serverThread.Start();
            //                Log.Information("启动成功！");

            //                //启动后执行
            //                ExecuteAfterStartup();

            //                while (true)
            //                {
            //                    var line = Console.ReadLine();
            //                    if (string.IsNullOrEmpty(line) || line.Equals("t", StringComparison.OrdinalIgnoreCase))
            //                    {
            //                        manualShutdown = true;
            //                        break;
            //                    }

            //                    if (line.Equals("r", StringComparison.OrdinalIgnoreCase))
            //                    {
            //                        Log.Information("服务器重启中。。。");
            //                        server.Restart();
            //                        Log.Information("重启完成!");
            //                    }
            //                }

            //                Log.Information("服务器停止中。。。");
            //                server.Stop();
            //                Log.Information("服务器已停止！");
            //            }
        }

        #region 方法

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="server"></param>
        static void StartServer(Server server)
        {
            Log.Information("服务器启动中。。。");
            Thread serverThread = new Thread(() => server.Start());
            serverThread.Start();
            Log.Information("启动成功！");

            //启动后执行
            ExecuteAfterStartup();

            while (true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line) || line.Equals("t", StringComparison.OrdinalIgnoreCase))
                {
                    manualShutdown = true;
                    break;
                }

                if (line.Equals("r", StringComparison.OrdinalIgnoreCase))
                {
                    Log.Information("服务器重启中。。。");
                    server.Restart();
                    Log.Information("重启完成!");
                }
            }

            Log.Information("服务器停止中。。。");
            server.Stop();
            Log.Information("服务器已停止！");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        static public async Task InitAsync()
        {
            LogManager.GetInstance();
            ConfigManager.LoadConfig();
            TokenManager.GetInstance();
            DatabaseManager.GetInstance();
            UserManager.GetInstance();

            await DatabaseManager.GetInstance().EnsureCreated();
            await DatabaseManager.GetInstance().GetUsers();
        }

        /// <summary>
        /// 启动后执行
        /// </summary>
        static public void ExecuteAfterStartup()
        {

        }

        /// <summary>
        /// 进程退出时重启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnProcessExit(object sender, EventArgs e)
        {
            if (!manualShutdown)
            {
                Log.Information("服务器自动重启中。。。");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Environment.GetCommandLineArgs()[0],
                    Arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1)),
                    UseShellExecute = true,
                    CreateNoWindow = false
                };
                Process.Start(startInfo);
            }
        }
        #endregion
    }
}