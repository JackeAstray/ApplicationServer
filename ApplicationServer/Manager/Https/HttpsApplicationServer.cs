using NetCoreServer;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ApplicationServer.Common.Base;

namespace ApplicationServer.Manager
{
    public class HttpsApplicationServer : HttpsServer, IApplicationServer
    {
        private Timer cleanupTimer;

        public HttpsApplicationServer(SslContext context, IPAddress address, int port) : base(context, address, port)
        {
            cleanupTimer = new Timer(CleanupInactiveSessions, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// 创建会话
        /// </summary>
        /// <returns></returns>
        protected override SslSession CreateSession()
        {
            return new HttpsApplicationSession(this);
        }

        /// <summary>
        /// 处理连接事件
        /// </summary>
        /// <param name="session"></param>
        protected override void OnConnected(SslSession session)
        {
            base.OnConnected(session);
            session.Socket.ReceiveTimeout = 120000; // 设置接收超时为120秒
            session.Socket.SendTimeout = 120000; // 设置发送超时为120秒
        }

        /// <summary>
        /// 处理服务器启动
        /// </summary>
        /// <param name="error"></param>
        protected override void OnError(SocketError error)
        {
            Log.Error($"HTTPS服务器捕获错误: {error}");
        }

        /// <summary>
        /// 清理无效会话
        /// </summary>
        /// <param name="state"></param>
        private void CleanupInactiveSessions(object state)
        {
            foreach (var session in Sessions.Values)
            {
                if (session is HttpsApplicationSession httpSession && httpSession.IsConnected && (DateTime.UtcNow - httpSession.LastActivity) > TimeSpan.FromMinutes(10))
                {
                    Log.Information($"清理无效会话: {httpSession.Id}, 最后活动时间: {httpSession.LastActivity}");
                    httpSession.Disconnect();
                }
            }
        }

        void IApplicationServer.Restart()
        {
            throw new NotImplementedException();
        }

        void IApplicationServer.Start()
        {
            throw new NotImplementedException();
        }

        void IApplicationServer.Stop()
        {
            throw new NotImplementedException();
        }
    }
}
