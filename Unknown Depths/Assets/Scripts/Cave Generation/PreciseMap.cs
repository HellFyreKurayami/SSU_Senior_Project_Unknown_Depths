using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

///Precise Map Class

///This class contains code references to both a tutorial located on
///the Unity site, and a tutorial found on LinkedIn Learning/Lynda by
///Sebstian Lague and Jesse Freeman Respectively

///https://unity3d.com/learn/tutorials/projects/procedural-cave-generation-tutorial/
///https://www.linkedin.com/learning/unity-5-2d-random-map-generation

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
        col = width;
        row = height;

        tiles = new PreciseTile[col * row];

        CreateTiles();
    }

    public void CreateTiles()
    {
        var Total = tiles.Length;
        for(var i = 0; i < Total; i++)
        {
            var tile = new PreciseTile();
            tile.TileID = i;
            tile.IsChecked = 0;
            tiles[i] = tile;
        }

        FindNeighbors();
    }

    public void CreateCave(int caveErode, int roomThresh)
    {
        var seed = Time.time.ToString();

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

    struct PreciseTileChecker
    {
        public int tileID;
        public int IsChecked;

        public PreciseTileChecker(int ID)
        {
            tileID = ID;
            IsChecked = 0;
        }
    }
    
    private void DetectRegions(int roomSize)
    {
        List<List<PreciseTileChecker>> caveRegions = GetRegions(0);
        foreach(List<PreciseTileChecker> cavern in caveRegions)
        {
            if(cavern.Count < roomSize)
            {
                foreach(PreciseTileChecker Tile in cavern)
                {
                    tiles[Tile.tileID].ClearNeighbors();
                    tiles[Tile.tileID].AutoTileID = (int)TilePiece.EMPTY;
                }
            }
        }
        Debug.Log("Finished Dectecting and Removing Regions");
    }

    private List<List<PreciseTileChecker>> GetRegions(int TileType)
    {
        List<List<PreciseTileChecker>> caverns = new List<List<PreciseTileChecker>>();
        for(var r = 0; r < row; r++)
        {
            for(var c = 0; c < col; c++)
            {
                if(tiles[col * r + c].AutoTileID >= TileType)
                {
                    List<PreciseTileChecker> newCavern = GetRegionTiles(col * r + c);
                    caverns.Add(newCavern);
                }
            }
        }
        Debug.Log("Exited For Loop in GetRegions()");
        return caverns;
    }

    private List<PreciseTileChecker> GetRegionTiles(int Tile)
    {
        List<PreciseTileChecker> caveTiles = new List<PreciseTileChecker>();

        Queue<PreciseTileChecker> queue = new Queue<PreciseTileChecker>();
        queue.Enqueue(new PreciseTileChecker(Tile));
        tiles[Tile].IsChecked = 1;

        while(queue.Count > 0)
        {
            PreciseTileChecker tile = queue.Dequeue();
            caveTiles.Add(tile);

            foreach(PreciseTile neighbor in tiles[Tile].Neighbors)
            {
                if(neighbor != null && neighbor.IsChecked == 0 && tiles[neighbor.TileID].AutoTileID >= 0)
                {
                    neighbor.IsChecked = 1;
                    queue.Enqueue(new PreciseTileChecker(neighbor.TileID));
                }
            }
        }
        return caveTiles;
    }
}
