using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Client : MonoBehaviour {

    public GameObject playMenu;
    public GameObject characterMenu;
    public GameObject textPlayer;
    public Text textEndPoint;
    public Text textName;
    public GameObject canvas;
    public Transform[] charaters;

    private Socket socket;
    private int myId;
    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    private List<GameObject> playersOrder = new List<GameObject>();
    private bool init = false;

    public void Update()
    {
        if (!init)
            return;
        SyncServer();
    }

    public bool Connect()
    {
        string ipString = textEndPoint.text;
        IPAddress ipAddress;
        if (ipString == "" || textName.text == "" || !IPAddress.TryParse(ipString, out ipAddress))
            return false;
        socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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
        Init();
        init = true;
    }

    public void Init()
    {
        byte[] buffer = new byte[4];
        socket.Receive(buffer);
        myId = BitConverter.ToInt32(buffer, 0);

        byte[] packet = MakePacket(sizeof(int) + textName.text.Length);
        BitConverter.GetBytes(textName.text.Length).CopyTo(packet, sizeof(int));
        System.Text.Encoding.ASCII.GetBytes(textName.text).CopyTo(packet, sizeof(int) * 2);
        socket.Send(packet);
    }

    public void SyncServer()
    {
        byte[] buffer = new byte[256];

        if (!socket.Poll(0, SelectMode.SelectRead))
            return;
        if (!socket.Connected)
            SceneManager.LoadScene("menu", LoadSceneMode.Single);
        socket.Receive(buffer, 4, SocketFlags.None);
        int id = BitConverter.ToInt32(buffer, 0);
        if (id == -1)
        {
            Game.playersName = new Dictionary<int, string>();
            foreach (var entry in players)
                Game.playersName.Add(entry.Key, entry.Value.GetComponent<Text>().text);
            Game.socket = socket;
            SceneManager.LoadScene("scene", LoadSceneMode.Single);
            return;
        }
        GameObject player;
        if (!players.TryGetValue(id, out player))
        {
            socket.Receive(buffer, 4, SocketFlags.None);
            int nameLength = BitConverter.ToInt32(buffer, 0);
            socket.Receive(buffer, nameLength, SocketFlags.None);
            string newPlayerName = System.Text.Encoding.ASCII.GetString(buffer);
            GameObject newPlayer = Instantiate(textPlayer, charaters[1].position, Quaternion.identity);
            newPlayer.GetComponent<Text>().text = newPlayerName;
            newPlayer.transform.SetParent(canvas.transform);
            newPlayer.transform.localScale = charaters[1].localScale;
            players.Add(id, newPlayer);
            playersOrder.Add(newPlayer);
            return;
        }
        socket.Receive(buffer, 1, SocketFlags.None);
        if (buffer[0] == 0xFF)
        {
            Destroy(players[id]);
            GameObject go = players[id];
            playersOrder.Remove(go);
            players.Remove(id);
            return;
        }
        player.transform.position = charaters[buffer[0]].position;
        player.transform.Translate(new Vector3(0, -(.3f + .3f * playersOrder.IndexOf(players[id])), 0));
    }

    public void Ready()
    {
        byte[] buffer = MakePacket(sizeof(byte));
        buffer[sizeof(int)] = 0xFF;
        socket.Send(buffer);
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
        BitConverter.GetBytes(myId).CopyTo(res, 0);
        return res;
    }
}
