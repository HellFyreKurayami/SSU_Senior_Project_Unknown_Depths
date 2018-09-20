using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RNG Map Test Class

//Most of this code is in refernece to a 2D Map
//Generation tutuorial found on LinkedIn Learning or
//Lynda.com. Tutorial by Jesse Freeman

//https://www.linkedin.com/learning/unity-5-2d-random-map-generation

public class RNGMapTest : MonoBehaviour {

    [Header("Map Dimensions")]
    public int mMapWidth = 20;
    public int mMapHeight = 20;

    public Map mMap;

	// Use this for initialization
	void Start () {
        mMap = new Map();
	}
	
    public void MakeMap()
    {
        mMap.NewMap(mMapWidth, mMapHeight);
        Debug.Log("Created a new map; " + mMap.mCol + " x " + mMap.mRow + " dimensions");
    }
}
