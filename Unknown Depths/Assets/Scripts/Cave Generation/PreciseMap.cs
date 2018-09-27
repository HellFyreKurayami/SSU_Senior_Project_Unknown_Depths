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

public class PreciseMap {

    public PreciseTile[] tiles; // Array of Tiles for the map
    public int col; //Map Size Columns/Length
    public int row; //Map Size Rows/Height

    public PreciseTile[] CoastTiles
    {
        get
        {
            return tiles.Where(t => t.AutoTileID < (int)TilePiece.GRASS).ToArray();
        }
    }

    public PreciseTile[] LandTiles
    {
        get
        {
            return tiles.Where(t => t.AutoTileID == (int)TilePiece.GRASS).ToArray();
        }
    }

    public void CreateMap(int width, int height)
    {
        row = width;// Will have variable names of r, x, i / Need to fix to be more uniform later
        col = height;// Will have variable names of c, y, j / Need to fix to be more uniform later

        tiles = new PreciseTile[row * col];

        CreateTiles();
    }

    public void CreateTiles()
    {
        var Total = tiles.Length;
        for(var i = 0; i < Total; i++)
        {
            var tile = new PreciseTile();
            tile.TileID = i;
            tiles[i] = tile;
        }

        FindNeighbors();
    }

    public void CreateCave(bool useSeed, string newSeed,int caveErode, int roomThresh)
    {
        var seed = "";
        if (useSeed)
        {
            seed = newSeed;
        }
        else
        {
            seed = Time.time.ToString();
        }

        System.Random psuRand = new System.Random(seed.GetHashCode());

        var total = tiles.Length;
        var MaxCol = col;
        var MaxRow = row;
        var Column = 0;
        var Row = 0;

        for(var i = 0; i < total; i++)
        {
            Column = i % MaxCol;
            if(Row == 0 || Row == MaxRow - 1 || Column == 0 || Column == MaxCol - 1)
            {
                tiles[i].ClearNeighbors();
                tiles[i].AutoTileID = (int)TilePiece.EMPTY;
            }
            else
            {
                if(psuRand.Next(0,100) < caveErode)
                {
                    tiles[i].ClearNeighbors();
                    tiles[i].AutoTileID = (int)TilePiece.EMPTY;
                }
            }

            if(Column == (MaxCol - 1))
            {
                Row++;
            }
        }

        DetectRegions(roomThresh);
    }

    private void FindNeighbors()
    {
        for (var r = 0; r < row; r++)
        {
            for (var c = 0; c < col; c++)
            {
                var tile = tiles[col * r + c];

                if (r < row - 1)
                {
                    tile.AddNeighbor(Sides.BOTTOM, tiles[col * (r + 1) + c]);
                }

                if (c < col - 1)
                {
                    tile.AddNeighbor(Sides.RIGHT, tiles[col * r + c + 1]);
                }

                if (c > 0)
                {
                    tile.AddNeighbor(Sides.LEFT, tiles[col * r + c - 1]);
                }

                if (r > 0)
                {
                    tile.AddNeighbor(Sides.TOP, tiles[col * (r - 1) + c]);
                }
            }
        }
    }

    /// <summary>
    /// Cavern class and respective methods below
    /// </summary>


    class Cavern
    {
        public List<PreciseTileChecker> tiles;
        public List<PreciseTileChecker> edges;
        public List<Cavern> connectedCaverns;
        public int caveSize;

        public Cavern()
        {
            //Literally does nothing, Congrats this is a useless constructor
        }

        public Cavern(List<PreciseTileChecker> caveTiles, PreciseTile[] map, int mapRow, int mapCol)
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
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            if (map[mapCol * x + y].AutoTileID >= 0)
                            {
                                edges.Add(tile);
                            }
                        }
                    }
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
    }

    private void ConnectCaverns(List<Cavern> Caves)
    {
        int closestDistance = 0;
        PreciseTileChecker bestTileA = new PreciseTileChecker();
        PreciseTileChecker bestTileB = new PreciseTileChecker();
        Cavern bestCaveA = new Cavern();
        Cavern bestCaveB = new Cavern();

        bool connectonFound = false;

        foreach(Cavern caveA in Caves)
        {
            connectonFound = false;
            foreach(Cavern caveB in Caves)
            {
                if(caveA == caveB)
                {
                    continue;
                }
                if (caveA.IsConnected(caveB))
                {
                    connectonFound = false;
                    break;
                }

                for(int tileIndexA = 0; tileIndexA < caveA.edges.Count; tileIndexA++)
                {
                    for(int tileIndexB = 0; tileIndexB < caveB.edges.Count; tileIndexB++)
                    {
                        PreciseTileChecker tileA = caveA.edges[tileIndexA];
                        PreciseTileChecker tileB = caveB.edges[tileIndexB];

                        int distance = (int)(Mathf.Pow((tileA.tileX - tileB.tileX), 2) + Mathf.Pow((tileA.tileY - tileB.tileY), 2));
                        if(distance < closestDistance || !connectonFound)
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

            if (connectonFound)
            {

                CreatePath(bestCaveA, bestCaveB, bestTileA, bestTileB);
            }
        }
    }

    private void CreatePath(Cavern caveA, Cavern caveB, PreciseTileChecker tileA, PreciseTileChecker tileB)
    {
        Cavern.ConnectCaves(caveA, caveB);
        Debug.Log("Tile A ID:" + tiles[col * tileA.tileX + tileA.tileY].TileID + " Tile B ID:" + tiles[col * tileB.tileX + tileB.tileY].TileID);
        //Debug.DrawLine(DisplayLine(tileA), DisplayLine(tileB), Color.green, 100);
    }

    Vector3 DisplayLine(PreciseTileChecker tile)
    {
        return new Vector3((tile.tileX * 16 + .5f), (-tile.tileY * 16 + .5f), 2);
    }

    /// <summary>
    /// Precise Tile Checker structure and needed methods below
    /// </summary>


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

    private void DetectRegions(int roomSize)
    {
        List<List<PreciseTileChecker>> caveRegions = GetRegions();
        List<Cavern> survivingRegions = new List<Cavern>();
        foreach (List<PreciseTileChecker> cavern in caveRegions)
        {
            //Debug.Log(cavern.Count);
            if(cavern.Count <= roomSize)
            {
                foreach(PreciseTileChecker Tile in cavern)
                {
                    tiles[col * Tile.tileX + Tile.tileY].ClearNeighbors();
                    tiles[col * Tile.tileX + Tile.tileY].AutoTileID = (int)TilePiece.EMPTY;
                }
            }
            else
            {
                survivingRegions.Add(new Cavern(cavern, tiles, row, col));
            }
        }

        Debug.Log("Going to connect regions now");
        ConnectCaverns(survivingRegions);
    }

    private List<List<PreciseTileChecker>> GetRegions()
    {
        List<List<PreciseTileChecker>> caverns = new List<List<PreciseTileChecker>>();
        int[,] mapFlags = new int[row, col];
        for(var r = 0; r < row; r++)
        {
            for(var c = 0; c < col; c++)
            {
                if(mapFlags[r, c] == 0 && tiles[col * r + c].AutoTileID >= (int)TileType.EMPTY && tiles[col * r + c] != null)
                {
                    List<PreciseTileChecker> newCavern = GetRegionTiles(r, c);
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

    private List<PreciseTileChecker> GetRegionTiles(int r, int c)
    {
        List<PreciseTileChecker> caveTiles = new List<PreciseTileChecker>();
        int[,] mapFlags = new int[row , col];

        Queue<PreciseTileChecker> queue = new Queue<PreciseTileChecker>();
        queue.Enqueue(new PreciseTileChecker(r, c));
        while (queue.Count > 0)
        {
            PreciseTileChecker tile = queue.Dequeue();
            if(tiles[col* r + c].AutoTileID > (int)TilePiece.EMPTY && tiles[col * r + c] != null)
            {
                caveTiles.Add(tile);
                mapFlags[r, c] = 1;
                for (var x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (var y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {

                        if (InsideMap(x, y) && (y == tile.tileY || x == tile.tileX))
                        {
                            if (mapFlags[x, y] == 0 && tiles[col * x + y].AutoTileID > -1)
                            {
                                mapFlags[x, y] = 1;
                                queue.Enqueue(new PreciseTileChecker(x, y));
                            }
                        }
                    }
                }
            }
        }
        return caveTiles;
    }

    private bool InsideMap(int x, int y)
    {
        return x >= 0 && x < row && y >= 0 && y < col;
    }
}
