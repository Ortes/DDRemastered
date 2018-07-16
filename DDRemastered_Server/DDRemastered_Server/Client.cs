using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace DDRemastered_Server
{
    class Client
    {
        private Socket socket;
        private bool gameStarted = false;
        private Mutex mutex = new Mutex();

        public Client(Socket socket)
        {
            this.socket = socket;
        }

        public void start()
        {
            byte[] buffer = new byte[1];
            while (!gameStarted)
            {
                Thread.Sleep(100);
                lock (mutex)
                {
                    if (socket.Poll(-1, SelectMode.SelectRead))
                        continue;
                    socket.Receive(buffer);
                }
            }
        }

        public void send()
        {
            lock (mutex)
            {

            }
        }
    }
}
