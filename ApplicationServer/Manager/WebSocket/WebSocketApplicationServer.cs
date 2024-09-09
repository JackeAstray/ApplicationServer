using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using ApplicationServer.Common.Base;

namespace ApplicationServer.Manager
{
    /// <summary>
    /// WebSocket应用服务器
    /// </summary>
    public class WebSocketApplicationServer : WsServer, IApplicationServer
    {
        public WebSocketApplicationServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new WebSocketApplicationSession(this); }

        protected override void OnError(SocketError error)
        {
            Log.Error($"WebSocketApplicationServer Error Code: {error}");
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
