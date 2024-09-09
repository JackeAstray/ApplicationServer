using ApplicationServer.Common.Base;
using NetCoreServer;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer.Manager.Udp
{
    public class UdpApplicationServer : UdsServer, IApplicationServer
    {
        public UdpApplicationServer(string path) : base(path) { }

        protected override UdsSession CreateSession() { return new UdpApplicationSession(this); }

        protected override void OnError(SocketError error)
        {
            Log.Error($"UdpApplicationServer Error Code: {error}");
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