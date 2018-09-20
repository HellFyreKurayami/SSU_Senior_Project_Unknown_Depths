using System.Collections;
using System.Collections.Generic;
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

    public int mID = 0; //Unique ID for each tile
    public Tile[] mNeighbors = new Tile[4]; //Tiles adjacent to this tile

}
