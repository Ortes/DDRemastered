using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public Board board;

	// Use this for initialization
	void Start () {
        GameObject BoardObject = new GameObject();
        BoardObject.transform.parent = transform;
        BoardObject.AddComponent<Board>();
        board.tilePrefab = (GameObject)Resources.Load("Prefabs/tile");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
