using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Map Class

//Most of this code is in refernece to a 2D Map
//Generation tutuorial found on LinkedIn Learning or
//Lynda.com. Tutorial by Jesse Freeman

//https://www.linkedin.com/learning/unity-5-2d-random-map-generation

public enum TileType
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

public class Map {

    public Tile[] tiles; // Array of Tiles for the map
    public int col; //Map Size Columns/Length
    public int row; //Map Size Rows/Height

    public Tile[] CoastTiles
    {
        get
        {
            return tiles.Where(t => t.AutoTileID < (int)TileType.GRASS).ToArray();
        }
    }

   public Tile[] LandTiles
    {
        get
        {
            return tiles.Where(t => t.AutoTileID == (int)TileType.GRASS).ToArray();
        }
    }

    public void NewMap(int width, int height)
    {
        col = width;
        row = height;

        tiles = new Tile[col * row]; // 1D Arry, but will be using math to traverse
                                        // the array as if it was a 2D Array

        CreateTiles();
    }

    public void CreateIsland(
        float erode,
        int erodeIter,
        float tree,
        float hill,
        float mountain,
        float town,
        float monster,
        float lake
    )
    {
        PopulateMap(LandTiles, lake, TileType.EMPTY);
        for(var i = 0; i < erodeIter; i++)
        {
            PopulateMap(CoastTiles, erode, TileType.EMPTY);
        }

        PopulateMap(LandTiles, tree, TileType.TREE);
        PopulateMap(LandTiles, hill, TileType.HILLS);
        PopulateMap(LandTiles, mountain, TileType.MOUNTAINS);
        PopulateMap(LandTiles, town, TileType.TOWNS);
        PopulateMap(LandTiles, monster, TileType.MONSTER);
    }

    private void CreateTiles()
    {
        var tTotal = tiles.Length; // Total Tiles
        for(var i = 0; i< tTotal; i++) // For each tile, create a new instance
        {                              // of the Tile class
            var tile = new Tile();
            tile.ID = i;
            tiles[i] = tile;
        }

        FindNeighbors();
    }

    private void FindNeighbors()
    {
        for(var r = 0; r < row; r++)
        {
            for(var c = 0; c < col; c++)
            {
                var tile = tiles[col * r + c];

                if(r < row - 1)
                {
                    tile.AddNeighbor(Sides.BOTTOM, tiles[col * (r + 1) + c]);
                }

                if(c < col - 1)
                {
                    tile.AddNeighbor(Sides.RIGHT, tiles[col * r + c + 1]);
                }

                if(c > 0)
                {
                    tile.AddNeighbor(Sides.LEFT, tiles[col * r + c - 1]);
                }

                if(r > 0)
                {
                    tile.AddNeighbor(Sides.TOP, tiles[col * (r - 1) + c]);
                }
            }
        }
    }

    public void PopulateMap(Tile[] tiles, float percent, TileType type)
    {
        var total = Mathf.FloorToInt(tiles.Length * percent);

        RandomizeMap(tiles);
        for (var i = 0; i < total; i++)
        {
            var tile = tiles[i];
            Debug.Log(tile.Neighbors.Length);
            if(type == TileType.EMPTY)
            {
                tile.ClearNeighbors();
            }
            tile.AutoTileID = (int)type;
        }
    }

    public void RandomizeMap(Tile[] tiles)
    {
        //Using the Fisher-Yates shuffling algorithm for map randomization
        //More information can be found here
        //https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

        for(var i = 0; i < tiles.Length; i++)
        {
            var tempTile = tiles[i];
            var r = Random.Range(i, tiles.Length);
            tiles[i] = tiles[r];
            tiles[r] = tempTile;
        }
    }
}
