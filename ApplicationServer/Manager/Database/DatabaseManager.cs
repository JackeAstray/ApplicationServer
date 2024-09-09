using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer.Manager
{
    /// <summary>
    /// 数据库管理
    /// </summary>
    public partial class DatabaseManager
    {
        #region 单例
        private static readonly Lazy<DatabaseManager> instance = new Lazy<DatabaseManager>(() => new DatabaseManager());
        public static DatabaseManager GetInstance() => instance.Value;
        #endregion

        #region 定时器
        private Timer? timer_15;
        private Timer? timer_30;
        private Timer? timer_60;
        #endregion

        #region 变量
        // 数据库连接字符串
        private readonly IServiceProvider serviceProvider;
        #endregion

        DatabaseManager()
        {
            serviceProvider = new ServiceCollection().
                AddDbContext<ApplicationDbContext>().
                BuildServiceProvider();
        }

        ~DatabaseManager()
        {

        }

        /// <summary>
        /// 确保数据库和表已创建
        /// </summary>
        /// <returns></returns>
        public async Task EnsureCreated()
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();
        }

        #region 用户
        /// <summary>
        /// 插入用户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public async Task<bool> InsertUserAsync(string username, string password, string email, int permissions)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var user = new User
                {
                    Username = username,
                    Password = password,
                    Email = email,
                    Permissions = (UserPermissions)permissions
                };

                context.User.Add(user);
                int rowsAffected = await context.SaveChangesAsync();

                if (rowsAffected > 0)
                {
                    // 获取新用户的索引
                    ulong userId = user.Id;
                    // 将新用户放入 User 字典
                    UserManager.GetInstance().AddUser(user);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error(e, "插入用户失败");
                return false;
            }
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public async Task GetUsers()
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            List<User> tempUserList = await context.User.ToListAsync();

            foreach (var user in tempUserList)
            {
                UserManager.GetInstance().AddUser(user);
            }
        }

        /// <summary>
        /// 通过ID获取用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User?> GetUserById(int id)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await context.User.FindAsync(id);
        }

        /// <summary>
        /// 通过用户名获取用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<User?> GetUserByUsername(string username)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await context.User.FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// 通过邮箱获取用户
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<User?> GetUserByEmail(string email)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await context.User.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task UpdateUser(User user)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.User.Update(user);
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task DeleteUser(User user)
        {
            await DeleteUser(user.Id);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteUser(ulong id)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await context.User.FindAsync(id);
            if (user != null)
            {
                context.User.Remove(user);
                await context.SaveChangesAsync();
            }
        }
        #endregion
    }
}