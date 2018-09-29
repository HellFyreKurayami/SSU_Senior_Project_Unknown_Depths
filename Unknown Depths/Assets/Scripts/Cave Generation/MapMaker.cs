using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Map Maker Class

///This class contains code references to both a tutorial located on
///the Unity site, and a tutorial found on LinkedIn Learning/Lynda by
///Sebstian Lague and Jesse Freeman Respectively

///https://unity3d.com/learn/tutorials/projects/procedural-cave-generation-tutorial/
///https://www.linkedin.com/learning/unity-5-2d-random-map-generation

public class MapMaker : MonoBehaviour {

    [Header("Map Dimensions")]
    public int MapWidth = 20;
    public int MapHeight = 20;

    [Space]
    [Header("Map Seed **Optional**")]
    public bool UseSeed = false;
    public string Seed = "";

    [Space]
    [Header("Visualize Map")]
    public GameObject MapContainer;
    public GameObject TilePrefab;
    public Vector2 TileSize = new Vector2(16, 16);

    [Space]
    [Header("Map Sprites")]
    public Texture2D MapTexture;

    [Space]
    [Header("Create Map")]
    [Range(10, 50)]
    public int caveErosion = 25;
    [Range(0, 15)]
    public int roomThreshold = 6;

    [Space]
    [Header("Populate Map")]
    [Range(0, 20)]
    public int treasureChests = 1;

    public PreciseMap Map;
    //public PreciseMap_Old Map;

    // Use this for initialization
    void Start () {
        Map = new PreciseMap();
        //Map = new PreciseMap_Old();
	}
	
    public void Create()
    {
        Map.CreateMap(MapWidth, MapHeight);
        //Debug.Log("Cave Created");
        Map.CreateCave(UseSeed, Seed, caveErosion, roomThreshold);
        CreateGrid();
    }

    void CreateGrid()
    {
        ClearMap();
        Sprite[] mSprites = Resources.LoadAll<Sprite>(MapTexture.name);

        var Total = Map.mapTiles.Length;
        var MaxColumns = Map.col;
        var Col = 0;
        var Row = 0;

        for (var i = 0; i < Total; i++)
        {
            Col = i % MaxColumns;

            var tNewX = Col * TileSize.x;
            var tNewY = -Row * TileSize.y;

            var go = Instantiate(TilePrefab);
            go.name = "Tile " + i;
            go.transform.SetParent(MapContainer.transform);
            go.transform.position = new Vector3(tNewX, tNewY, 0);

            var tile = Map.mapTiles[i];
            var spriteID = tile.AutoTileID;

            if (spriteID >= 0)
            {
                var sr = go.GetComponent<SpriteRenderer>();
                sr.sprite = mSprites[spriteID];
            }

            if (Col == (MaxColumns - 1))
            {
                Row++;
            }
        }
    }

    void ClearMap()
    {
        var children = MapContainer.transform.GetComponentsInChildren<Transform>();
        for (var i = children.Length - 1; i > 0; i--)
        {
            Destroy(children[i].gameObject);
        }
    }
}
