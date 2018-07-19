using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UI;
using UnityEngine;
using System.Threading;


public class Client : MonoBehaviour {

    public GameObject playMenu;
    public GameObject characterMenu;
    public GameObject textPlayer;
    public Text textEndPoint;
    public Text textName;
    public Button[] charaters;

    private Socket socket;
    private int id;
    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    public bool Connect()
    {
        string ipString = textEndPoint.text;
        if (ipString == "" || textName.text == "")
            return false;
        IPAddress ipAddress;
        if (!IPAddress.TryParse(ipString, out ipAddress))
            return false;
        socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.ReceiveTimeout = 1000;
        socket.Connect(ipAddress, 65174);
        if (!socket.Connected)
            return false;
        return true;
    }

    public void Play()
    {
        if (!Connect())
            return;
        playMenu.SetActive(false);
        characterMenu.SetActive(true);
        new Thread(Run).Start();
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
        Init();
        byte[] buffer = new byte[256];
        bool startGame = false;

        while (!startGame)
        {
            Thread.Sleep(50);
            lock (socket)
            {
                if (!socket.Poll(-1, SelectMode.SelectRead))
                    continue;

                socket.Receive(buffer);
                int id = BitConverter.ToInt32(buffer, 0);
                if (id == -1 && (startGame = true))
                    continue;
                if (buffer[sizeof(int)] == 0xFF)
                {
                    Destroy(players[id]);
                    players.Remove(id);
                    continue;
                }
                GameObject player;
                if (!players.TryGetValue(id, out player))
                {
                    int nameLength = BitConverter.ToInt32(buffer, sizeof(int));
                    string newPlayerName = System.Text.Encoding.ASCII.GetString(buffer, 2 * sizeof(int), nameLength);
                    GameObject newPlayer = Instantiate(textPlayer, charaters[1].transform.position, Quaternion.identity);
                    newPlayer.GetComponent<Text>().text = newPlayerName;
                    newPlayer.transform.Translate(new Vector3(0, id * .2f, 0));
                    players.Add(id, newPlayer);
                    continue;
                }
                player.transform.position = charaters[buffer[sizeof(int)]].transform.position;
                player.transform.Translate(new Vector3(0, id * .2f, 0));
            }
        }
    }

    public void SendChoice(int choice)
    {
        byte[] buffer = MakePacket(sizeof(byte));
        buffer[sizeof(int)] = (byte)choice;
        socket.Send(buffer);
    }

    public byte[] MakePacket(int size)
    {
        byte[] res = new byte[sizeof(int) + size];
        BitConverter.GetBytes(id).CopyTo(res, 0);
        return res;
    }
}
