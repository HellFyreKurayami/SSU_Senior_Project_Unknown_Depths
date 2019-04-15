using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

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

    public static MapMaker Instance { get; set; }

    [System.Serializable]
    public struct EnemySpawns
    {
        [SerializeField] public List<Entity> enemies;
    }

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

    [Space]
    [Header("Battle Information")]
    public List<Entity> Players = null;
    //public List<Entity> Enemies = null;
    public List<EnemySpawns> EnemySpawnData;
    public List<Entity> Bosses;

    private Dictionary<int, string> gameMaps = new Dictionary<int, string>();
    public PreciseMap Map;

    private BattleWindow battleWindow;
    private FloorWindow floorWindow;
    private ProgressWindow progressWindow;
    [System.NonSerialized]
    public int floor = 1;

    private int tempX;
    private int tempY;
    private Sprite[] caveTileSprites;
    private Sprite[] fowTileSprites;
    // Use this for initialization

    public WindowManager windowManager
    {
        get
        {
            return GenericWindow.manager;
        }
    }

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        MapContainer.SetActive(true);
        Directory.Delete(Application.persistentDataPath + "/PlayerInfo/", true);
        foreach (Entity p in Players)
        {
            p.SaveData();
        }
    }

    public void Reset()
    {
        Start();
        caveTileSprites = Resources.LoadAll<Sprite>(MapTexture.name);
        fowTileSprites = Resources.LoadAll<Sprite>(fowTexture.name);

        Map = new PreciseMap();
        Create();
        StartCoroutine(AddPlayer(false));
    }

    public void Shutdown()
    {
        ClearMap();
    }

    IEnumerator AddPlayer(bool revisit)
    {
        yield return new WaitForEndOfFrame();
        CreatePlayer(revisit);
    }
	
    public void Create()
    {
        string seed = Time.time.ToString();
        Map.CreateMap(MapWidth + (5*floor - 1), MapHeight + (5 * floor - 1));
        //Debug.Log(string.Format("Map Width: {0} | Map Height {1}", MapWidth + (5 * floor - 1), MapHeight + (5 * floor - 1)));
        Map.CreateCave(seed, caveErosion, roomThreshold, treasureChests);
        CreateGrid();
        if(floor == 1)
        {
            CenterMap(Map.StartingTile.TileID);
        }
        else
        {
            CenterMap(Map.caveEntranceTile.TileID);
        }
        gameMaps.Add(floor, seed);
    }

    private void Revisit(string seed)
    {
        Map.CreateMap(MapWidth + (5 * floor - 1), MapHeight + (5 * floor - 1));
        //Debug.Log(string.Format("Map Width: {0} | Map Height {1}", MapWidth + (5 * floor - 1), MapHeight + (5 * floor - 1)));
        //Debug.Log("Cave Created");
        Map.CreateCave(seed, caveErosion, roomThreshold, 0);
        CreateGrid();
        CenterMap(Map.caveExitTile.TileID);
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

    public void CreatePlayer(bool revisit)
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

        if (revisit)
        {
            controller.MoveTo(Map.caveExitTile.TileID);
        }
        else
        {
            if(floor == 1)
            {
                controller.MoveTo(Map.StartingTile.TileID);
            }
            else
            {
                controller.MoveTo(Map.caveEntranceTile.TileID);
            }
        }

        //Display Floor Stats
        floorWindow = windowManager.Open((int)Windows.FloorWindow - 1, false) as FloorWindow;
        floorWindow.UpdateFloor(floor);
    }

    bool hasMoved = false;
    bool yn = false;
    void TileActionCallback(int type)
    {
        //Debug.Log("On Tile Type: " + type);
        var tileID = player.GetComponent<MapMovement>().currentTile;
        VisitTile(tileID);
        if (player.GetComponent<MapMovement>().currentTile.Equals(Map.caveExitTile.TileID))
        {
            if (hasMoved)
            {
                progressWindow = windowManager.Open((int)Windows.ProgressWindow - 1, false) as ProgressWindow;
                progressWindow.NextOrPrevious(true);
                yn = true;
                ToggleMovement(false);
            }
        }
        else if (floor != 1)
        {
            if (player.GetComponent<MapMovement>().currentTile.Equals(Map.caveEntranceTile.TileID))
            {
                Debug.Log(string.Format("Floor: {0}", floor));
                if (hasMoved)
                {
                    progressWindow = windowManager.Open((int)Windows.ProgressWindow - 1, false) as ProgressWindow;
                    progressWindow.NextOrPrevious(false);
                    yn = false;
                    ToggleMovement(false);
                }
            }
        }
        else if(type == 20) // Treasure Chest
        {

        }
        else
        {
            var chance = Random.Range(0, 1f);
            //Debug.Log(Map.caveEntranceTile.TileID);
            if(Map.caveEntranceTile == null)
            {
                if(chance < 0.15f && !player.GetComponent<MapMovement>().currentTile.Equals(Map.caveExitTile.TileID))
                {
                    //Debug.Log("Battle Starting");
                    MapContainer.SetActive(false);
                    StartBattle(false);
                }
            }
            else
            {
                if(chance < 0.15f && !player.GetComponent<MapMovement>().currentTile.Equals(Map.caveEntranceTile.TileID) && !player.GetComponent<MapMovement>().currentTile.Equals(Map.caveExitTile.TileID))
                {
                    //Debug.Log("Battle Starting");
                    MapContainer.SetActive(false);
                    StartBattle(false);
                }
            }
        }
        hasMoved = true;
    }

    public void Proceed()
    {
        if (yn)
        {
            if (gameMaps.ContainsKey(floor + 1))
            {
                floor++;
                Revisit(gameMaps[floor]);
                StartCoroutine(AddPlayer(false));
                //Debug.Log("Revisiting the Next Floor. Seed: " + gameMaps[floor]);
            }
            else
            {
                if(floor == 10)
                {
                    MapContainer.SetActive(false);
                    StartBattle(true);
                }
                else
                {
                    floor++;
                    Reset();
                }
                //Debug.Log("Going to Next Floor. Seed: " + gameMaps[floor]);
            }
        }
        else
        {
            floor--;
            Revisit(gameMaps[floor]);
            StartCoroutine(AddPlayer(true));
            //Debug.Log("Going to Previous Floor Seed: " + gameMaps[floor]);
            ToggleMovement(true);
            progressWindow.Close();
        }
        hasMoved = false;
        ToggleMovement(true);
        progressWindow.Close();
    }

    public void Stay()
    {
        hasMoved = false;
        ToggleMovement(true);
        progressWindow.Close();
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

    //Battle Functions

    public void StartBattle(bool boss)
    {
        battleWindow = windowManager.Open((int)Windows.BattleWindow - 1, false) as BattleWindow;
        battleWindow.battleOverCall += BattleOver;

        if (boss)
        {
            battleWindow.StartBattleBoss(Players, Bosses, (floor/10));
        }
        else
        {
            battleWindow.StartBattle(Players, EnemySpawnData, floor);
            battleWindow.UpdateCharUI();
        }

        ToggleMovement(false);
    }

    public void EndBattle()
    {
        battleWindow.Close();
        ToggleMovement(true);
    }

    public void ToggleMovement(bool state)
    {
        player.GetComponent<Player>().isActive = state;
        player.GetComponent<MapMovement>().enabled = state;
        Camera.main.GetComponent<MoveCamera>().enabled = state;
    }

    private void BattleOver(bool playerWin)
    {
        if (!playerWin)
        {
            StartCoroutine(ExitGame(false));
        }
        else
        {
            if(floor%10 == 0)
            {
                StartCoroutine(ExitGame(true));
            }
            MapContainer.SetActive(true);
            EndBattle();
        }
    }

    IEnumerator ExitGame(bool win)
    {
        gameMaps.Clear();
        yield return new WaitForSeconds(5);
        if (win)
        {
            windowManager.Open((int)Windows.CreditsWindow - 1, true);
        }
        windowManager.Open((int)Windows.CreditsWindow - 1, true);
    }
}
