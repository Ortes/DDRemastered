using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject tilePrefab;
    public int size;
    public Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();

    // Use this for initialization
    void Start()
    {
        SpawnTiles();
        SpawnPlayer();
    }

    void SpawnTiles()
    {
        size = 8;
        int idTile = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
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

    void SpawnPlayer()
    {
        GameObject playerObject = (GameObject)Instantiate(playerPrefab);
        Player player = playerObject.AddComponent<Player>();
        playerObject.transform.parent = transform;
        playerObject.name = "Guigui";
        player.coordinates = new Vector2(0, 0);
        playerObject.transform.position = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
