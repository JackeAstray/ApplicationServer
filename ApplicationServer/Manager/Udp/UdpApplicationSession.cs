using NetCoreServer;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer.Manager
{
    public class UdpApplicationSession : UdsSession
    {
        public UdpApplicationSession(UdsServer server) : base(server) { }

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat Unix Domain Socket session with Id {Id} connected!");

            // Send invite message
            string message = "Hello from Unix Domain Socket chat! Please send a message or '!' to disconnect the client!";
            SendAsync(message);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat Unix Domain Socket session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);

            // Multicast message to all connected sessions
            Server.Multicast(message);

            // If the buffer starts with '!' the disconnect the current session
            if (message == "!")
                Disconnect();
        }

        protected override void OnError(SocketError error)
        {
            Log.Error($"UdpApplicationSession Error Code: {error}");
        }
    }
}
