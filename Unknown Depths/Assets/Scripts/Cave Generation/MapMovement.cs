using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Map Movement Class
/// 
/// Most of this code is in reference to
/// tutorials found on LinkedinLearning/Lynda
/// Tutorial by Jesse Freeman
/// 
/// https://www.linkedin.com/learning/unity-5-2d-movement-in-an-rpg-game
/// </summary>

public class MapMovement : MonoBehaviour {

    public PreciseMap map;
    public Vector2 tileSize;
    public int currentTile;
    public float speed = 1.0f;
    public bool moving;
    public int[] blockedTileTypes;
    public delegate void TileAction(int type);
    public TileAction TileActionCallback;
    public delegate void MoveAction();
    public MoveAction MoveActionCallback;

    private float dt;
    private Vector2 startPos, endPos;
    private int tempX;
    private int tempY;
    private int tempIndex;


    public void MoveTo(int index, bool animate = false)
    {
        if (!CanMove(index))
        {
            return;
        }

        if(MoveActionCallback != null)
        {
            MoveActionCallback();
        }
        currentTile = index;

        PositionUtil.CalcPosition(index, map.row, out tempX, out tempY);

        tempX *= (int)tileSize.x;
        tempY *= -(int)tileSize.y;

        var newPos = new Vector3(tempX, tempY, 0);
        if (!animate)
        {
            transform.position = newPos;

            if(TileActionCallback != null)
            {
                TileActionCallback(map.mapTiles[currentTile].AutoTileID);
            }
        }
        else
        {
            startPos = transform.position;
            endPos = newPos;
            dt = 0;
            moving = true;
        }
    }

    public void MoveInDir(Vector2 dir)
    {
        PositionUtil.CalcPosition(currentTile, map.row, out tempX, out tempY);

        tempX += (int)dir.x;
        tempY += (int)dir.y;

        PositionUtil.CalcIndex(tempX, tempY, map.row, out tempIndex);

        //Debug.Log("Moving to: " + tempIndex);
        MoveTo(tempIndex, true);
    }

    private void Update()
    {
        if (moving)
        {
            dt += Time.deltaTime;
            if (dt > speed)
            {
                moving = false;
                transform.position = endPos;
                if (TileActionCallback != null)
                {
                    TileActionCallback(map.mapTiles[currentTile].AutoTileID);
                }
            }

            transform.position = Vector2.Lerp(startPos, endPos, dt / speed);
        }
    }

    private bool CanMove(int index)
    {

        if(index < 0 || index >= map.mapTiles.Length)
        {
            return false;
        }

        var tileType = map.mapTiles[index].AutoTileID;
        if (moving || Array.IndexOf(blockedTileTypes, tileType) > -1)
        {
            return false;
        }
        return true;
    }
}
