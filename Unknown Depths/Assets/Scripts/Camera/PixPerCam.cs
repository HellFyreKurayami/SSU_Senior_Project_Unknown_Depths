using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PixPerCam Class

//Most of this code is in refernece to a 2D Map
//Generation tutuorial found on LinkedIn Learning or
//Lynda.com. Tutorial by Jesse Freeman

//https://www.linkedin.com/learning/unity-5-2d-random-map-generation

public class PixPerCam : MonoBehaviour {


    public static float pixToUnit = 1f;
    public static float scale = 1;

    public Vector2 nativeRes = new Vector2(160, 144);

    private void Awake()
    {
        var camera = GetComponent<Camera>();

        if (camera.orthographic)
        {
            var dir = Screen.height;
            var res = nativeRes.y;

            scale = dir / res;
            pixToUnit *= scale;

            camera.orthographicSize = (dir / 2.0f) / pixToUnit;
        }
    }
}
