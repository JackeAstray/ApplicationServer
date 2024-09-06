using Serilog;

namespace ApplicationServer.Manager
{
    /// <summary>
    /// 日志管理器
    /// </summary>
    public class LogManager
    {
        private static LogManager? instance;
        public static LogManager GetInstance()
        {
            if (instance == null)
            {
                instance = new LogManager();
            }

            return instance;
        }

        LogManager()
        {
            Log.Logger = new LoggerConfiguration().
                      MinimumLevel.Verbose().
                      WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code).
                      WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day).
                      CreateLogger();
        }
    }
}
