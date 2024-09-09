using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer
{
    public class Config
    {
        public int Port { get; set; }
        public string? Secret { get; set; }
        public ConnectionMode ConnectionMode { get; set; }
        //数据库连接字符串
        public ConnectionStrings ConnectionStrings { get; set; } = null!;
    }

    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public class ConnectionStrings
    {
        //数据库
        public string Database { get; set; } = null!;
        //任务连接
        public string TarefasConnection { get; set; } = null!;
    }
}
