using ApplicationServer.Manager;
using Serilog;

namespace ApplicationServer
{
    internal class Program
    {
        private const string Version = "0.1.2";
        private static bool manualShutdown = false;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        static public async Task InitAsync()
        {
            LogManager.GetInstance();
        }

        /// <summary>
        /// 启动后执行
        /// </summary>
        static public void ExecuteAfterStartup()
        {

        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}