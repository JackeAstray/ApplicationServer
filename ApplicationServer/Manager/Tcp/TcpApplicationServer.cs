using NetCoreServer;
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
    public class TcpApplicationServer : TcpServer, IApplicationServer
    {
        public TcpApplicationServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new TcpApplicationSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP server caught an error with code {error}");
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
