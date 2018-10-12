using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Position Utility Class
/// 
/// Most of this code is in reference to a tutorial found on
/// LinkedIn Learning / Lynda. Tutorial by Jesse Freeman
/// </summary>

public class PositionUtil{
    
    public static void CalcIndex(int x, int y, int width, out int index)
    {
        index = x + y * width;
    }

    public static void CalcPosition(int index, int width, out int x, out int y)
    {
        x = index % width;
        y = index / width;
    }
}
