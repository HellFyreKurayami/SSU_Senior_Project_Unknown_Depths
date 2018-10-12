using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private MapMovement moveController;

	// Use this for initialization
	void Start () {
        moveController = GetComponent<MapMovement>();
	}
	
	// Update is called once per frame
	void Update () {
        var dir = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            dir.y = -1;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            dir.x = -1;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            dir.y = 1;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            dir.x = 1;
        }

        if(dir.x != 0 || dir.y != 0)
        {
            moveController.MoveInDir(dir);
        }
    }
}
