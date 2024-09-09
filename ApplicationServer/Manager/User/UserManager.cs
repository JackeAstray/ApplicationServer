using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer.Manager
{
    public class UserResult
    {
        public int resultCode { get; set; }

        public string? resultMessage { get; set; }
    }

    public class UserManager
    {
        private static UserManager? instance;
        public static UserManager GetInstance()
        {
            if (instance == null)
            {
                instance = new UserManager();
            }

            return instance;
        }

        Dictionary<string, User> users = new Dictionary<string, User>();

        UserManager()
        {

        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        public void AddUser(User user)
        {
            if (!users.ContainsKey(user.Username))
            {
                Log.Debug("添加用户：{0}", user.Username);
                users.Add(user.Username, user);
            }
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetUser(string username)
        {
            if (users.ContainsKey(username))
            {
                return users[username];
            }

            return null;
        }

        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateUser(string username, string password)
        {
            if (users.ContainsKey(username))
            {
                return users[username].Password == password;
            }

            return false;
        }
    }
}