using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer
{
    public static class ConfigManager
    {
        private static Config? config;

        public static void LoadConfig()
        {
            // 构建Config.json文件的路径
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "Config.json");
            // 读取JSON文件
            var json = File.ReadAllText(configFilePath);
            // 解析JSON文件
            config = JsonConvert.DeserializeObject<Config>(json);
        }

        public static Config GetConfig()
        {
            if (config == null)
            {
                Log.Error("配置尚未加载。请先调用LoadConfig方法。");
                return null;
            }

            return config;
        }
    }
}
