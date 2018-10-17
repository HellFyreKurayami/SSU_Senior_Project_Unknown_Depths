using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///Map Maker Class
///
///This class contains code references to both a tutorial located on
///the Unity site, and a tutorial found on LinkedIn Learning/Lynda by
///Sebstian Lague and Jesse Freeman Respectively
///
///https://unity3d.com/learn/tutorials/projects/procedural-cave-generation-tutorial/
///https://www.linkedin.com/learning/unity-5-2d-random-map-generation 
///https://www.linkedin.com/learning/unity-5-2d-movement-in-an-rpg-game
/// </summary>

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
    public Texture2D fowTexture;

    [Space]
    [Header("Player")]
    public GameObject playerPrefab;
    public GameObject player;
    public int viewDistance = 3;

    [Space]
    [Header("Create Map")]
    [Range(20, 50)]
    public int caveErosion = 25;
    [Range(0, 15)]
    public int roomThreshold = 6;

    [Space]
    [Header("Populate Map")]
    [Range(0, 10)]
    public int treasureChests = 1;

    public PreciseMap Map;

    private int tempX;
    private int tempY;
    private Sprite[] caveTileSprites;
    private Sprite[] fowTileSprites;
    // Use this for initialization
    void Start () {
        caveTileSprites = Resources.LoadAll<Sprite>(MapTexture.name);
        fowTileSprites = Resources.LoadAll<Sprite>(fowTexture.name);

        Reset();
    }

    public void Reset()
    {
        Map = new PreciseMap();
        Create();
        StartCoroutine(AddPlayer());
    }

    IEnumerator AddPlayer()
    {
        yield return new WaitForEndOfFrame();
        CreatePlayer();
    }
	
    public void Create()
    {
        Map.CreateMap(MapWidth, MapHeight);
        //Debug.Log("Cave Created");
        Map.CreateCave(UseSeed, Seed, caveErosion, roomThreshold, treasureChests);
        CreateGrid();
        CenterMap(Map.caveEntranceTile.TileID);
    }

    void CreateGrid()
    {
        ClearMap();
        
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

            DecorateTile(i);

            if (Col == (MaxColumns - 1))
            {
                Row++;
            }
        }
    }

    private void DecorateTile(int tileID)
    {
        var tile = Map.mapTiles[tileID];
        var spriteID = tile.AutoTileID;
        var go = MapContainer.transform.GetChild(tileID).gameObject;

        if (spriteID >= 0)
        {
            var sr = go.GetComponent<SpriteRenderer>();
            if (tile.visited)
            {
                sr.sprite = caveTileSprites[spriteID];
            }
            else
            {
                tile.CalcFOWAutoTileID();
                sr.sprite = fowTileSprites[Mathf.Min(tile.fowAutoTileID, fowTileSprites.Length-1)];
            }
        }
    }

    public void CreatePlayer()
    {
        player = Instantiate(playerPrefab);
        player.name = "Player";
        player.transform.SetParent(MapContainer.transform);

        var controller = player.GetComponent<MapMovement>();
        controller.map = Map;
        controller.tileSize = TileSize;
        controller.TileActionCallback += TileActionCallback;

        var moveScript = Camera.main.GetComponent<MoveCamera>();
        moveScript.target = player;

        controller.MoveTo(Map.caveEntranceTile.TileID);
    }

    void TileActionCallback(int type)
    {
        //Debug.Log("On Tile Type: " + type);
        var tileID = player.GetComponent<MapMovement>().currentTile;
        VisitTile(tileID);
    }

    void ClearMap()
    {
        var children = MapContainer.transform.GetComponentsInChildren<Transform>();
        for (var i = children.Length - 1; i > 0; i--)
        {
            Destroy(children[i].gameObject);
        }
    }

    void CenterMap(int index)
    {
        var camPos = Camera.main.transform.position;
        var width = Map.row; //May need to change to Map.row

        PositionUtil.CalcPosition(index, width, out tempX, out tempY);

        camPos.x = tempX * TileSize.x;
        camPos.y = -(tempY * TileSize.y);
        Camera.main.transform.position = camPos;
    }

    void VisitTile(int index)
    {
        int column, newX, newY, row = 0;
        PositionUtil.CalcPosition(index, Map.col, out tempX, out tempY);
        var half = Mathf.FloorToInt(viewDistance / 2f);
        tempX -= half;
        tempY -= half;

        var total = viewDistance * viewDistance;
        var maxCol = viewDistance - 1;

        for(int i = 0; i < total; i++)
        {
            column = i % viewDistance;

            newX = column + tempX;
            newY = row + tempY;

            PositionUtil.CalcIndex(newX, newY, Map.col, out index);
            if(index > -1 && index < Map.mapTiles.Length)
            {
                var tile = Map.mapTiles[index];
                tile.visited = true;
                DecorateTile(index);

                foreach(var neighbor in tile.Neighbors)
                {
                    if(neighbor != null)
                    {
                        if (!neighbor.visited)
                        {
                            neighbor.CalcFOWAutoTileID();
                            DecorateTile(neighbor.TileID);
                        }
                    }
                }
            }

            if(column == maxCol)
            {
                row++;
            }
        }
    }
}
