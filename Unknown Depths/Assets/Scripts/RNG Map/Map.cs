using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Map Class

//Most of this code is in refernece to a 2D Map
//Generation tutuorial found on LinkedIn Learning or
//Lynda.com. Tutorial by Jesse Freeman

//https://www.linkedin.com/learning/unity-5-2d-random-map-generation

public class Map {

    public Tile[] mTiles; // Array of Tiles for the map
    public int mCol; //Map Size Columns/Length
    public int mRow; //Map Size Rows/Height

    public void NewMap(int mWidth, int mHeight)
    {
        mCol = mWidth;
        mRow = mHeight;

        mTiles = new Tile[mCol * mRow]; //1D Arry, but will be using math to traverse
                                        //the array as if it was a 2D Array

        CreateTiles();
    }

    private void CreateTiles()
    {
        var tTotal = mTiles.Length;
        for(var i = 0; i< tTotal; i++)
        {
            var mTile = new Tile();
            mTile.mID = i;
            mTiles[i] = mTile;
        }
    }
}
