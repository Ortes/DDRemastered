using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace DDRemastered_Server
{
    class Server
    {
        private int port;

        public Server(int port)
        {
            this.port = port;
        }

        public void start()
        {
            IPAddress ipAddress = IPAddress.Any;
            Socket server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);

            server.Bind(endPoint);

            server.Listen(10);
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                server.Accept();
            }
        }
    }
}
