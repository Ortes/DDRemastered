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
        private string name;
        private Socket socket;
        private bool gameStarted = false;
        private bool isOK = false;
        private Server server;
        private byte character = 1;

        public Client(Socket socket, Server server)
        {
            this.socket = socket;
            this.server = server;
            id = currentId;
            ++currentId;
        }

        public void Init()
        {
            socket.Send(BitConverter.GetBytes(id));

            byte[] buffer = new byte[256];
            int size = socket.Receive(buffer);
            Send(buffer, size);
            server.Broadcast(buffer, size);
            server.SendAllInit(socket);

            int nameLength = BitConverter.ToInt32(buffer, sizeof(int));
            name = System.Text.Encoding.ASCII.GetString(buffer, 2 * sizeof(int), nameLength);
            Console.WriteLine("New player: " + name);
        }

        public void Start()
        {
            byte[] buffer = new byte[256];
            while (!gameStarted)
            {
                Thread.Sleep(50);
                lock (socket)
                {
                    if (!socket.Poll(0, SelectMode.SelectRead))
                        continue;
                    try
                    {
                        socket.Receive(buffer);
                    }
                    catch
                    {
                        server.RemovePlayer(this);
                        Console.WriteLine("Player " + name + " disconnect from " + socket.RemoteEndPoint);
                        socket.Close();
                        break;
                    }
                    if (buffer[0] == 0xFF)
                    {
                        isOK = true;
                        server.PlayerOK();
                    }
                    else
                    {
                        server.ChangeCharacter(this, buffer[sizeof(int)]);
                        character = buffer[sizeof(int)];
                    }
                }
            }
            socket.Close();
        }

        public void Send(byte[] buffer)
        {
            lock (socket)
                socket.Send(buffer);
        }

        public void Send(byte[] buffer, int size)
        {
            lock (socket)
                socket.Send(buffer, size, SocketFlags.None);
        }

        public byte[] GetInitPacket()
        {
            return PacketMaker.MakeInit(id, name);
        }

        public byte[] GetCharacterPacket()
        {
            return PacketMaker.MakeChCharacter(id, character);
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
