using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {


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
