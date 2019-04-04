using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// 
///Precise Map Class
///This class contains code references to both a tutorial located on
///the Unity site, and a tutorial found on LinkedIn Learning/Lynda by
///Sebstian Lague and Jesse Freeman Respectively
///
///https://unity3d.com/learn/tutorials/projects/procedural-cave-generation-tutorial/
///https://www.linkedin.com/learning/unity-5-2d-random-map-generation
///
/// </summary>

public enum TilePiece
{
    EMPTY = -1,
    GRASS = 15,
    TREE = 16,
    HILLS = 17,
    MOUNTAINS = 18,
    TOWNS = 19,
    CASTLE = 20,
    MONSTER = 21
}

public class PreciseMap
{

    public PreciseTile[] mapTiles;
    public int[,] mapLayout;
    public int row; //Can also be conisdered to be the X value
    public int col; //Can also be considered to be the Y value

    public PreciseTile[] CoastTiles
    {
        get
        {
            return mapTiles.Where(t => t.AutoTileID < (int)TilePiece.GRASS).ToArray();
        }
    }

    public PreciseTile[] LandTiles
    {
        get
        {
            return mapTiles.Where(t => t.AutoTileID == (int)TilePiece.GRASS).ToArray();
        }
    }

    public PreciseTile caveEntranceTile
    {
        get
        {
            return mapTiles.FirstOrDefault(t => t.AutoTileID == (int)TilePiece.MONSTER);
        }
    }

    public PreciseTile caveExitTile
    {
        get
        {
            return mapTiles.FirstOrDefault(t => t.AutoTileID == (int)TilePiece.TREE);
        }
    }

    public void CreateMap(int width, int height)
    {
        row = width; // X
        col = height; // Y

        mapTiles = new PreciseTile[row * col];
        mapLayout = new int[row, col];

        CreateTiles();
    }

    public void CreateTiles()
    {
        var Total = mapTiles.Length;
        for (var i = 0; i < Total; i++)
        {
            var tile = new PreciseTile();
            tile.TileID = i;
            mapTiles[i] = tile;
        }

        FindNeighbors();
    }

    private void FindNeighbors()
    {
        for (var x = 0; x < row; x++)
        {
            for (var y = 0; y < col; y++)
            {
                var tile = mapTiles[col * x + y];

                if (x < row - 1)
                {
                    tile.AddNeighbor(TileSides.BOTTOM, mapTiles[col * (x + 1) + y]);
                }

                if (y < col - 1)
                {
                    tile.AddNeighbor(TileSides.RIGHT, mapTiles[col * x + y + 1]);
                }

                if (y > 0)
                {
                    tile.AddNeighbor(TileSides.LEFT, mapTiles[col * x + y - 1]);
                }

                if (x > 0)
                {
                    tile.AddNeighbor(TileSides.TOP, mapTiles[col * (x - 1) + y]);
                }
            }
        }
    }

    public void CreateCave(string newSeed, int caveErode, int roomThresh, int chests)
    {
        var seed = newSeed;
        

        System.Random psuRand = new System.Random(seed.GetHashCode());

        for (var x = 0; x < row; x++)
        {
            for (var y = 0; y < col; y++)
            {
                if (x == 0 || x == row - 1 || y == 0 || y == col - 1)
                {
                    mapLayout[x, y] = 0;
                }
                else
                {
                    mapLayout[x, y] = (psuRand.Next(0, 100) < caveErode) ? 0 : 1;
                }
            }
        }

        DetectRegions(roomThresh);

        for(var x = 0; x < row; x++)
        {
            for(var y = 0; y < col; y++)
            {
                if(mapLayout[x, y] == 0)
                {
                    mapTiles[col * x + y].ClearNeighbors();
                    mapTiles[col * x + y].AutoTileID = (int)TilePiece.EMPTY;
                }
            }
        }
        addItems(2, chests);
    }

    private void addItems(int doors, int chests)
    {
        List<PreciseTileChecker> renderedTiles = new List<PreciseTileChecker>();
        for(var x = 0; x < row; x++)
        {
            for (var y = 0; y < col; y++)
            {
                if(mapTiles[col * x + y].AutoTileID > 0)
                {
                    renderedTiles.Add(new PreciseTileChecker(x, y));
                }
            }
        }
        
        for(var d = 0; d < doors; d++)
        {
            PreciseTileChecker tile = renderedTiles[UnityEngine.Random.Range(0, renderedTiles.Count)];
            if(d == 0)
            {
                mapTiles[col * tile.tileX + tile.tileY].AutoTileID = (int)TilePiece.MONSTER;
            }
            else
            {
                mapTiles[col * tile.tileX + tile.tileY].AutoTileID = (int)TilePiece.TREE;
            }
            renderedTiles.Remove(tile);
        }

        for(var c = 0; c < chests; c++)
        {
            PreciseTileChecker tile = renderedTiles[UnityEngine.Random.Range(0, renderedTiles.Count)];
            mapTiles[col * tile.tileX + tile.tileY].AutoTileID = (int)TilePiece.CASTLE;
            renderedTiles.Remove(tile);
        }
    }

    private void DetectRegions(int caveSize)
    {
        int previousRoomCount = 0;
        while(previousRoomCount != 1)
        {
            //Debug.Log("Passing through");
            List<List<PreciseTileChecker>> caveRegions = GetRegions();
            List<Cavern> survivingRegions = new List<Cavern>();
            foreach (List<PreciseTileChecker> cavern in caveRegions)
            {
                //Debug.Log("Cavern Number: "+id+" Cavern Size: "+cavern.Count+" Parent Tile: "+(col*cavern[0].tileX+cavern[0].tileY));
                if (cavern.Count <= caveSize + 1)
                {
                    foreach (PreciseTileChecker Tile in cavern)
                    {
                        mapLayout[Tile.tileX, Tile.tileY] = 0;
                    }
                }
                else
                {
                    survivingRegions.Add(new Cavern(cavern, mapLayout, row, col));
                }
            }
            previousRoomCount = survivingRegions.Count();
            ConnectCaverns(survivingRegions);
       }
        
    }

    private List<List<PreciseTileChecker>> GetRegions()
    {
        List<List<PreciseTileChecker>> caverns = new List<List<PreciseTileChecker>>();
        int[,] mapFlags = new int[row, col];

        for(var x = 0; x < row; x++)
        {
            for(var y = 0; y < col; y++)
            {
                if(mapFlags[x,y] == 0 && mapLayout[x,y] == 1)
                {
                    List<PreciseTileChecker> newCavern = GetRegionTiles(x, y);
                    caverns.Add(newCavern);

                    foreach(PreciseTileChecker tile in newCavern)
                    {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }
        return caverns;
    }

    private List<PreciseTileChecker> GetRegionTiles(int x, int y)
    {
        List<PreciseTileChecker> caveTiles = new List<PreciseTileChecker>();
        int[,] mapFlags = new int[row, col];

        Queue<PreciseTileChecker> queue = new Queue<PreciseTileChecker>();
        queue.Enqueue(new PreciseTileChecker(x, y));
        while(queue.Count > 0)
        {
            PreciseTileChecker tile = queue.Dequeue();

            if(mapTiles[col*x+y].AutoTileID > (int)TilePiece.EMPTY && mapTiles[col*x+y] != null)
            {
                caveTiles.Add(tile);
                for (var r = tile.tileX - 1; r <= tile.tileX + 1; r++)
                {
                    for (var c = tile.tileY - 1; c <= tile.tileY + 1; c++)
                    {
                        if (IsInMapRange(r, c) && (c == tile.tileY || r == tile.tileX))
                        {
                            if (mapFlags[r, c] == 0 && mapLayout[r, c] == 1)
                            {
                                mapFlags[r, c] = 1;
                                queue.Enqueue(new PreciseTileChecker(r, c));
                            }
                        }
                    }
                }
            }
        }

        return caveTiles;
    }

    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < row && y >= 0 && y < col;
    }

    struct PreciseTileChecker
    {
        public int tileX;
        public int tileY;

        public PreciseTileChecker(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    class Cavern : IComparable<Cavern>
    {
        public List<PreciseTileChecker> tiles;
        public List<PreciseTileChecker> edges;
        public List<Cavern> connectedCaverns;
        public int caveSize;

        public bool isAccessibleFromMainCavern;
        public bool isMainCavern = false;

        public Cavern()
        {
            //Literally does nothing, Congrats this is a useless constructor
        }

        public Cavern(List<PreciseTileChecker> caveTiles, int[,] map, int Row, int Col)
        {
            //This is the real constructor
            tiles = caveTiles; //List of tiles in the cavern we are checking
            caveSize = tiles.Count; //How many tiles we have to check
            connectedCaverns = new List<Cavern>(); //Contains rooms this object is connected to

            edges = new List<PreciseTileChecker>();
            foreach (PreciseTileChecker tile in tiles)
            {
                for (var x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (var y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if ((x == tile.tileX || y == tile.tileY) && x >= 0 && x < Row && y >= 0 && y < Col)
                        {
                            if (map[x, y] == 1)
                            {
                                //Debug.Log("Adding Edge");
                                edges.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public void SetAccessibleFromMainCavern()
        {
            if (!isAccessibleFromMainCavern)
            {
                isAccessibleFromMainCavern = true;
                foreach (Cavern connectedCave in connectedCaverns)
                {
                    connectedCave.SetAccessibleFromMainCavern();
                }
            }
        }

        public static void ConnectCaves(Cavern caveA, Cavern caveB)
        {
            caveA.connectedCaverns.Add(caveB);
            caveB.connectedCaverns.Add(caveA);
        }

        public bool IsConnected(Cavern other)
        {
            return connectedCaverns.Contains(other);
        }

        public int CompareTo(Cavern other)
        {
            return other.caveSize.CompareTo(caveSize);
        }
    }

    private void ConnectCaverns(List<Cavern> Caves, bool forceAccessibilityFromMain = false)
    {
        List<Cavern> caveListA = new List<Cavern>();
        List<Cavern> caveListB = new List<Cavern>();

        if (forceAccessibilityFromMain)
        {
            foreach (Cavern cave in Caves)
            {
                if (cave.isAccessibleFromMainCavern)
                {
                    caveListB.Add(cave);
                }
                else
                {
                    caveListA.Add(cave);
                }
            }
        }
        else
        {
            caveListA = Caves;
            caveListB = Caves;
        }
        int closestDistance = 0;
        PreciseTileChecker bestTileA = new PreciseTileChecker();
        PreciseTileChecker bestTileB = new PreciseTileChecker();
        Cavern bestCaveA = new Cavern();
        Cavern bestCaveB = new Cavern();

        bool connectonFound = false;

        foreach (Cavern caveA in caveListA)
        {
            if (!forceAccessibilityFromMain)
            {
                connectonFound = false;
                if (caveA.connectedCaverns.Count > 0)
                {
                    continue;
                }
            }

            foreach (Cavern caveB in caveListB)
            {
                if (caveA == caveB || caveA.IsConnected(caveB))
                {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < caveA.edges.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < caveB.edges.Count; tileIndexB++)
                    {
                        PreciseTileChecker tileA = caveA.edges[tileIndexA];
                        PreciseTileChecker tileB = caveB.edges[tileIndexB];

                        int distance = (int)(Mathf.Pow((tileA.tileX - tileB.tileX), 2) + Mathf.Pow((tileA.tileY - tileB.tileY), 2));

                        if (distance < closestDistance || !connectonFound)
                        {
                            closestDistance = distance;
                            connectonFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestCaveA = caveA;
                            bestCaveB = caveB;
                        }
                    }
                }
            }

            if (connectonFound && !forceAccessibilityFromMain)
            {
                CreatePath(bestCaveA, bestCaveB, bestTileA, bestTileB);
            }
        }

        if(connectonFound && forceAccessibilityFromMain)
        {
            CreatePath(bestCaveA, bestCaveB, bestTileA, bestTileB);
            ConnectCaverns(Caves, true);
        }

        if (!forceAccessibilityFromMain)
        {
            ConnectCaverns(Caves, true);
        }

    } // <3

    private void CreatePath(Cavern caveA, Cavern caveB, PreciseTileChecker tileA, PreciseTileChecker tileB)
    {
        Cavern.ConnectCaves(caveA, caveB);
        //Debug.Log("Tile A ID:" + mapTiles[col * tileA.tileX + tileA.tileY].TileID + " Tile B ID:" + mapTiles[col * tileB.tileX + tileB.tileY].TileID);
        //Debug.Log("Tile A X:" + tileA.tileX + " Tile A Y:" + tileA.tileY + " Tile B X:" + tileB.tileX + " Tile B Y:" + tileB.tileY);
        //Debug.Log("Begin Line");
        List<PreciseTileChecker> line = GetPassageTiles(tileA, tileB);
        foreach (PreciseTileChecker t in line)
        {
            //Debug.Log("Tile X: " + t.tileX + " Tile Y: " + t.tileY + " Tile ID: " + (col * t.tileX + t.tileY));
            DrawPassage(t, 1);
        }
        //Debug.Log("End Line");
    }

    private void DrawPassage(PreciseTileChecker t, int r)
    {
        for(var x = -r; x <= r; x++)
        {
            for(var y = -r; y <= r; y++)
            {
                //Debug.Log("Attempting to draw passage");
                if(x*x + y*y <= r * r)
                {
                    //Debug.Log("Drawing passage");
                    int drawX = t.tileX + x;
                    int drawY = t.tileY + y;
                    if(IsInMapRange(drawX, drawY))
                    {
                        mapLayout[drawX, drawY] = 1;
                    }
                }
            }
        }
    }

    private List<PreciseTileChecker> GetPassageTiles(PreciseTileChecker from, PreciseTileChecker to)
    {
        List<PreciseTileChecker> line = new List<PreciseTileChecker>();

        int x = from.tileX;
        int y = from.tileY;
        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;
        bool inverted = false;
        int step = Math.Sign(dx);
        int gradient = Math.Sign(dy);
        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);
            step = Math.Sign(dy);
            gradient = Math.Sign(dx);
        }

        int gradientAcc = longest / 2;
        for (var i = 0; i < longest; i++)
        {
            line.Add(new PreciseTileChecker(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAcc += shortest;

            if (gradientAcc >= longest)
            {
                if (inverted)
                {
                    x += gradient;
                }
                else
                {
                    y += gradient;
                }

                gradientAcc -= longest;
            }
            //Debug.Log("X = " + x + " Y = " + y);
        }
        return line;
    }
}
