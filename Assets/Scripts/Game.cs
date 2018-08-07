using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class Game : MonoBehaviour {

    public static Socket socket;
    public static Dictionary<int, string> playersName;

	// Use this for initialization
	void Start () {
        GameObject BoardObject = new GameObject();
        BoardObject.transform.parent = transform;
        Board board = BoardObject.AddComponent<Board>();
        board.tilePrefab = (GameObject)Resources.Load("Prefabs/Tile");
        board.playerPrefab = (GameObject)Resources.Load("Prefabs/Player");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
