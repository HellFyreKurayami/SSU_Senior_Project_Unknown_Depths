using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//Tile Class

//Most of this code is in refernece to a 2D Map
//Generation tutuorial found on LinkedIn Learning or
//Lynda.com. Tutorial by Jesse Freeman

//https://www.linkedin.com/learning/unity-5-2d-random-map-generation

public enum Sides
{
    BOTTOM,
    RIGHT,
    LEFT,
    TOP
}
public class Tile {

    public int ID = 0; //Unique ID for each tile
    public Tile[] Neighbors = new Tile[4]; //Tiles adjacent to this tile
    public int AutoTileID;
    
    public void AddNeighbor(Sides side, Tile tile)
    {
        Neighbors[(int)side] = tile;
        CalcAutoTileID();
    }

    public void RemoveNeighbor(Tile tile)
    {
        var total = Neighbors.Length;
        for(var i = 0; i < total; i++)
        {
            if(Neighbors[i] != null)
            {
                if(Neighbors[i].ID == tile.ID)
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
        for(var i = 0; i < total; i++)
        {
            var tile = Neighbors[i];
            if(tile != null)
            {
                tile.RemoveNeighbor(this);
                Neighbors[i] = null;
            }
        }

        CalcAutoTileID();
    }

    private void CalcAutoTileID()
    {
        var sidesValues = new StringBuilder();
        foreach(Tile tile in Neighbors)
        {
            sidesValues.Append(tile == null ? "0" : "1");
        }

        AutoTileID = System.Convert.ToInt32(sidesValues.ToString(), 2);
    }
}
