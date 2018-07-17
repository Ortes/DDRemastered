using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    public GameObject tilePrefab;
    public int size;
    public Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();

    // Use this for initialization
    void Start () {
		
	}

    void SpawnTiles () {
        int idTile = 0;
        for (int i =0; i<size; i++)
        {
            for(int j=0; j<size; j++)
            {
                GameObject tileObject = (GameObject)Instantiate(tilePrefab);
                Tile tile = tileObject.AddComponent<Tile>();
                tileObject.transform.parent = transform;
                tileObject.name = "Tile " + idTile;
                tile.coordinates = new Vector2(i, j);
                tileObject.transform.position = new Vector3(i, 0, j);
                idTile++;

            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
