using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace DDRemastered_Server
{
    class Server
    {
        private readonly int port;
        private List<Client>[] characterSlots;

        public Server(int port)
        {
            this.port = port;
            characterSlots = new List<Client>[5];
            for (int i = 0; i < characterSlots.Length; i++)
                characterSlots[i] = new List<Client>();
        }

        public void Start()
        {
            IPAddress ipAddress = IPAddress.Any;
            Socket server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);

            server.Bind(endPoint);
            server.Listen(10);
            Console.WriteLine("Waiting for a connection...");

            while (true)
            {
                Socket newSocket = server.Accept();
                Console.WriteLine(((IPEndPoint)newSocket.RemoteEndPoint).Address.ToString() + " connected");

                Client newClient = new Client(newSocket, this);
                newClient.Init();
                characterSlots[1].Add(newClient);
                new Thread(newClient.Start).Start();
            }
        }

        public void SendAllInit(Socket socket)
        {
            lock (characterSlots)
                for (int i = 0; i < characterSlots.Length; i++)
                    characterSlots[i].ForEach(x => { socket.Send(x.GetInitPacket()); socket.Send(x.GetCharacterPacket()); });
        }

        public void ChangeCharacter(Client client, int index)
        {
            lock (characterSlots)
            {
                Broadcast(PacketMaker.MakeChCharacter(client.GetId(), (byte)index));
                for (int i = 0; i < characterSlots.Length; i++)
                    if (characterSlots[i].Remove(client))
                        break;
                characterSlots[index].Add(client);
            }
        }

        public void Broadcast(byte[] buffer)
        {
            lock (characterSlots)
                for (int i = 0; i < characterSlots.Length; i++)
                    characterSlots[i].ForEach(x => x.Send(buffer));
        }

        public void Broadcast(byte[] buffer, int size)
        {
            lock (characterSlots)
                for (int i = 0; i < characterSlots.Length; i++)
                    characterSlots[i].ForEach(x => x.Send(buffer, size));
        }

        public void RemovePlayer(Client client)
        {
            lock (characterSlots)
            {
                for (int i = 0; i < characterSlots.Length; i++)
                    if (characterSlots[i].Remove(client))
                        break;
                Broadcast(PacketMaker.MakeDestroy(client.GetId()));
            }
        }

        public void PlayerOK()
        {
            lock (characterSlots)
            {
                for (int i = 0; i < characterSlots.Length; i++)
                    if (characterSlots[i].Exists(x => !x.IsOK()))
                        break;
                Console.WriteLine("Game starting...");
                Broadcast(PacketMaker.MakeReady());
            }
        }
    }
}
        