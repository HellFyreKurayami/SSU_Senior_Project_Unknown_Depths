using System.Collections;
using System.Collections.Generic;
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
            tiles[i] = tile;
        }

        FindNeighbors();
    }

    public void CreateCave(int caveErode, int smoothIter)
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
        for (var i = 0; i < smoothIter; i++)
        {
            SmoothCave();
        }
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

    private void SmoothCave()
    {
        var total = tiles.Length;

        for(var i = 0; i < total; i++)
        {
            var tile = tiles[i];
            var nCount = 0;

            for(var j = 0; j < tile.Neighbors.Length; j++)
            {
                if (tile.Neighbors[j] != null)
                {
                    nCount++;
                }
            }
            if (nCount > 2)
            {
                tiles[i].ClearNeighbors();
                tiles[i].AutoTileID = (int)TilePiece.EMPTY;
            }
        }
    }
}
