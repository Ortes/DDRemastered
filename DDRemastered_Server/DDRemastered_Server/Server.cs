﻿using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace DDRemastered_Server
{
    class Server
    {
        private int port;
        private List<Client>[] characterSlots;

        public Server(int port)
        {
            this.port = port;
            nbPlayers = 0;
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
                Client newClient = new Client(server.Accept(), this);
                characterSlots[1].Add(newClient);
                new Thread(newClient.Start).Start();
            }
        }

        public void ChangeClass(Client client, int index)
        {
            lock (characterSlots)
            {
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

        public void RemovePlayer(Client client)
        {
            lock (characterSlots)
                for (int i = 0; i < characterSlots.Length; i++)
                    if (characterSlots[i].Remove(client))
                        break;
        }

        public void PlayerOK()
        {
            lock (characterSlots)
            {
                for (int i = 0; i < characterSlots.Length; i++)
                    if (characterSlots[i].Exists(x => !x.IsOK()))
                        break;
                byte[] buffer = new byte[1];
                buffer[0] = 0xFF;
                Broadcast(buffer);
            }
        }
    }
}
