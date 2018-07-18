using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UI;
using UnityEngine;
using System.Threading;


public class Client : MonoBehaviour {

    public GameObject emptyMenu;
    public Text textEndPoint;
    public Text textName;

    private Socket socket;
    private int id;
    private Dictionary<int, string> players = new Dictionary<int, string>();

    public bool Connect()
    {
        string ipString = textEndPoint.text.Split(':')[0];
        if (ipString == "" || textName.text == "")
            return false;
        IPAddress ipAddress;
        if (!IPAddress.TryParse(ipString, out ipAddress))
            return false;
        socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.ReceiveTimeout = 1000;
        socket.Connect(ipAddress, Int32.Parse(ipString));
        if (!socket.Connected)
            return false;
        return true;
    }

    public void Init()
    {
        byte[] buffer = new byte[256];
        BitConverter.GetBytes(textName.text.Length).CopyTo(buffer, 0);
        System.Text.Encoding.ASCII.GetBytes(textName.text).CopyTo(buffer, sizeof(int));
        socket.Send(buffer, sizeof(int) + textName.text.Length, SocketFlags.None);
        socket.Receive(buffer);
        id = BitConverter.ToInt32(buffer, 0);
    }

    public void Run()
    {
        byte[] buffer = new byte[256];
        bool startGame = false;

        while (startGame)
        {
            Thread.Sleep(50);
            lock (socket)
            {
                if (socket.Poll(-1, SelectMode.SelectRead))
                    continue;

                socket.Receive(buffer);
                int id = BitConverter.ToInt32(buffer, 0);
                if (id == -1 && (startGame = true))
                    continue;
                if (buffer[sizeof(int)] == 0xFF)
                    players.Remove(id);
                string playerName = players[id];
                if (playerName == null)
                {
                    int nameLength = BitConverter.ToInt32(buffer, sizeof(int));
                    string newPlayerName = System.Text.Encoding.ASCII.GetString(buffer, 2 * sizeof(int), nameLength);
                    players.Add(id, newPlayerName);
                }
            }
        }
    }

    public byte[] MakePacket(int size)
    {
        byte[] res = new byte[sizeof(int) + size];
        BitConverter.GetBytes(id).CopyTo(res, 0);
        return res;
    }
}
