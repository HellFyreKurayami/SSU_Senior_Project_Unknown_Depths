using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RNG Map Test Class

//Most of this code is in refernece to a 2D Map
//Generation tutuorial found on LinkedIn Learning or
//Lynda.com. Tutorial by Jesse Freeman

//https://www.linkedin.com/learning/unity-5-2d-random-map-generation

public class RNGMapTest : MonoBehaviour {

    [Header("Map Dimensions")]
    public int MapWidth = 20;
    public int MapHeight = 20;

    [Space]
    [Header("Visualize Map")]
    public GameObject MapContainer;
    public GameObject TilePrefab;
    public Vector2 TileSize = new Vector2(16, 16);

    [Space]
    [Header("Map Sprites")]
    public Texture2D MapTexture;

    [Space]
    [Header("Populate Map")]
    [Range(0, .9f)]
    public float erodePercent = .5f;
    public int erodeIter = 2;
    [Range(0, .9f)]
    public float treePercent = .3f;
    [Range(0, .9f)]
    public float hillPercent = .2f;
    [Range(0, .9f)]
    public float mountainPercent = .1f;
    [Range(0, .9f)]
    public float townPercent = .05f;
    [Range(0, .9f)]
    public float monsterPercent = .1f;
    [Range(0, .9f)]
    public float lakePercent = .05f;

    public Map Map;

	// Use this for initialization
	void Start () {
        Map = new Map();
	}
	
    public void MakeMap()
    {
        Map.NewMap(MapWidth, MapHeight);
        Map.CreateIsland(
            erodePercent,
            erodeIter,
            treePercent,
            hillPercent,
            mountainPercent,
            townPercent,
            monsterPercent,
            lakePercent
        );
        CreateGrid();
    }

    void CreateGrid()
    {
        ClearMap();
        Sprite[] mSprites = Resources.LoadAll<Sprite>(MapTexture.name);

        var Total = Map.tiles.Length;
        var MaxColumns = Map.col;
        var Col = 0;
        var Row = 0;

        for(var i = 0; i<Total; i++)
        {
            Col = i % MaxColumns;

            var tNewX = Col * TileSize.x;
            var tNewY = -Row * TileSize.y;

            var go = Instantiate(TilePrefab);
            go.name = "Tile " + i;
            go.transform.SetParent(MapContainer.transform);
            go.transform.position = new Vector3(tNewX, tNewY, 0);

            var tile = Map.tiles[i];
            var spriteID = tile.AutoTileID;

            if (spriteID >= 0)
            {
                var sr = go.GetComponent<SpriteRenderer>();
                sr.sprite = mSprites[spriteID];
            }

            if(Col == (MaxColumns - 1))
            {
                Row++;
            }
        }
    }

    void ClearMap()
    {
        var children = MapContainer.transform.GetComponentsInChildren<Transform>();
        for(var i = children.Length - 1; i > 0; i--)
        {
            Destroy(children[i].gameObject);
        }
    }
}
