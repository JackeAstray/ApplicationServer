using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer.Manager
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        //账户
        public DbSet<User> User { get; set; }

        private readonly string connectionString;
        private MySqlServerVersion serverVersion;

        public ApplicationDbContext()
        {
            this.connectionString = ConfigManager.GetConfig().ConnectionStrings.TarefasConnection;
        }

        /// <summary>
        /// 配置数据库
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            this.serverVersion = new MySqlServerVersion(new Version(5, 7, 44));
            optionsBuilder.UseMySql(connectionString, serverVersion, options =>
            {
                options.MaxBatchSize(100); // 设置批处理大小
                options.CommandTimeout(60); // 设置命令超时时间
            });
        }
    }
}
