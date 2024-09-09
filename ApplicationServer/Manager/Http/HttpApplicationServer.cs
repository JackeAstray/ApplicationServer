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
    public class HttpApplicationServer : HttpServer, IApplicationServer
    {
        private Timer cleanupTimer;

        public HttpApplicationServer(IPAddress address, int port) : base(address, port)
        {
            cleanupTimer = new Timer(CleanupInactiveSessions, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        protected override TcpSession CreateSession()
        {
            return new HttpApplicationSession(this);
        }

        /// <summary>
        /// 已连接
        /// </summary>
        /// <param name="session"></param>
        protected override void OnConnected(TcpSession session)
        {
            base.OnConnected(session);
            session.Socket.ReceiveTimeout = 120000; // 设置接收超时为120秒
            session.Socket.SendTimeout = 120000; // 设置发送超时为120秒
        }

        /// <summary>
        /// 服务器关闭
        /// </summary>
        /// <param name="error"></param>
        protected override void OnError(SocketError error)
        {
            Log.Error($"HTTP服务器捕获错误: {error}");
        }

        /// <summary>
        /// 清理无效会话
        /// </summary>
        /// <param name="state"></param>
        private void CleanupInactiveSessions(object state)
        {
            foreach (var session in Sessions.Values)
            {
                if (session is HttpApplicationSession httpSession && httpSession.IsConnected && (DateTime.UtcNow - httpSession.LastActivity) > TimeSpan.FromMinutes(10))
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