using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

///<summary>
///Precise Tile Class
///
///This class contains code references to both a tutorial located on
///the Unity site, and a tutorial found on LinkedIn Learning/Lynda by
///Sebstian Lague and Jesse Freeman Respectively
///
///https://unity3d.com/learn/tutorials/projects/procedural-cave-generation-tutorial/
///https://www.linkedin.com/learning/unity-5-2d-random-map-generation
///</summary>


public enum TileSides
{
    BOTTOM,
    RIGHT,
    LEFT,
    TOP
}

public class PreciseTile{

    public int TileID = 0; //Unique ID for each tile object.
    public PreciseTile[] Neighbors = new PreciseTile[4]; //Contains all tiles adjacent to calling tile object
    public int AutoTileID; //Contains a reference to the sprite called on render

    public void AddNeighbor(TileSides side, PreciseTile tile)
    {
        Neighbors[(int)side] = tile;
        CalcAutoTileID();
    }

    public void RemoveNeighbor(PreciseTile tile)
    {
        var total = Neighbors.Length;
        for (var i = 0; i < total; i++)
        {
            if (Neighbors[i] != null)
            {
                if (Neighbors[i].TileID == tile.TileID)
                {
                    Neighbors[i] = null;
                }
            }
        }

        CalcAutoTileID();
    }

    public void ClearNeighbors()
    {
        var total = Neighbors.Length;
        for (var i = 0; i < total; i++)
        {
            var tile = Neighbors[i];
            if (tile != null)
            {
                tile.RemoveNeighbor(this);
                Neighbors[i] = null;
            }
        }

        CalcAutoTileID();
    }

    public void CalcAutoTileID()
    {
        var sidesValues = new StringBuilder();
        foreach (PreciseTile tile in Neighbors)
        {
            sidesValues.Append(tile == null ? "0" : "1");
        }

        AutoTileID = System.Convert.ToInt32(sidesValues.ToString(), 2);
    }
}
