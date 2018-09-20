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

    [Space]
    [Header("Visualize Map")]
    public GameObject mMapContainer;
    public GameObject mTilePrefab;
    public Vector2 mTileSize = new Vector2(16, 16);

    [Space]
    [Header("Map Sprites")]
    public Texture2D mMapTexture;

    public Map mMap;

	// Use this for initialization
	void Start () {
        mMap = new Map();
	}
	
    public void MakeMap()
    {
        mMap.NewMap(mMapWidth, mMapHeight);
        Debug.Log("Created a new map; " + mMap.mCol + " x " + mMap.mRow + " dimensions");
        CreateGrid();
    }

    void CreateGrid()
    {
        ClearMap();
        Sprite[] mSprites = Resources.LoadAll<Sprite>(mMapTexture.name);

        var tTotal = mMap.mTiles.Length;
        var mMaxColumns = mMap.mCol;
        var tCol = 0;
        var tRow = 0;

        for(var i = 0; i<tTotal; i++)
        {
            tCol = i % mMaxColumns;

            var tNewX = tCol * mTileSize.x;
            var tNewY = -tRow * mTileSize.y;

            var go = Instantiate(mTilePrefab);
            go.name = "Tile " + i;
            go.transform.SetParent(mMapContainer.transform);
            go.transform.position = new Vector3(tNewX, tNewY, 0);

            var spriteID = 0;
            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = mSprites[spriteID];

            if(tCol == (mMaxColumns - 1))
            {
                tRow++;
            }
        }
    }

    void ClearMap()
    {
        var children = mMapContainer.transform.GetComponentsInChildren<Transform>();
        for(var i = children.Length - 1; i > 0; i--)
        {
            Destroy(children[i].gameObject);
        }
    }
}
