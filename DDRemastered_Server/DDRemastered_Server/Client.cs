using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace DDRemastered_Server
{
    class Client
    {
        static private int currentId = 0;

        private int id;
        private Socket socket;
        private bool gameStarted = false;
        private String name;
        private bool isOK = false;
        private Server server;

        public Client(Socket socket, Server server)
        {
            this.socket = socket;
            this.server = server;
            id = currentId;
            ++currentId;
        }

        public void Init()
        {
            byte[] buffer = new byte[256];
            socket.Receive(buffer);
            int nameLength = BitConverter.ToInt32(buffer, 0);
            name = System.Text.Encoding.ASCII.GetString(buffer, sizeof(int), nameLength);
            socket.Send(BitConverter.GetBytes(id));
            server.Broadcast(PacketMaker.MakeInit(id, name));
        }

        public void Start()
        {
            byte[] buffer = new byte[1];
            while (!gameStarted)
            {
                Thread.Sleep(100);
                lock (socket)
                {
                    if (socket.Poll(-1, SelectMode.SelectRead))
                        continue;
                    if (socket.Receive(buffer) == 0)
                    {
                        server.RemovePlayer(this);
                        break;
                    }
                    if (buffer[0] == 0xFF)
                    {
                        isOK = true;
                        server.PlayerOK();
                    }
                    else
                        server.ChangeCharacter(this, buffer[0]);
                }
            }
            socket.Close();
        }

        public void Send(byte[] buffer)
        {
            lock (socket)
                socket.Send(buffer);
        }

        public void StartGame()
        {
            gameStarted = true;
        }

        public bool IsOK()
        {
            return isOK;
        }

        public int GetId()
        {
            return id;
        }
    }
}
