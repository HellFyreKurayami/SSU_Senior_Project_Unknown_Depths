using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMovement : MonoBehaviour {

    public PreciseMap map;
    public Vector2 tileSize;
    public int currentTile;

    private int tempX;
    private int tempY;
    private int tempIndex;

    public void MoveTo(int index)
    {
        currentTile = index;

        PositionUtil.CalcPosition(index, map.row, out tempX, out tempY);

        tempX *= (int)tileSize.x;
        tempY *= -(int)tileSize.y;

        transform.position = new Vector3(tempX, tempY, 0);
    }

    public void MoveInDir(Vector2 dir)
    {
        PositionUtil.CalcPosition(currentTile, map.row, out tempX, out tempY);

        tempX += (int)dir.x;
        tempY += (int)dir.y;

        PositionUtil.CalcIndex(tempX, tempY, map.row, out tempIndex);

        Debug.Log("Moving to: " + tempIndex);
    }
}
