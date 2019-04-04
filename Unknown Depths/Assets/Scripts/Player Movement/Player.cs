using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Class
/// 
/// Most of this code is in reference to
/// tutorials found on LinkedinLearning/Lynda
/// Tutorial by Jesse Freeman
/// 
/// https://www.linkedin.com/learning/unity-5-2d-movement-in-an-rpg-game
/// </summary>

public class Player : MonoBehaviour {

    private MapMovement moveController;
    private Animator animator;
    public bool hasMoved = false;
	// Use this for initialization
	void Start () {
        moveController = GetComponent<MapMovement>();
        moveController.MoveActionCallback += OnMove;
        moveController.TileActionCallback += OnTile;

        animator = GetComponent<Animator>();
        animator.speed = 0;
	}

    private void OnMove()
    {
        animator.speed = 1;
    }

    private void OnTile(int type)
    {
        animator.speed = 0;
    }

    // Update is called once per frame
    void Update () {
        var dir = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            dir.y = -1;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            dir.x = -1;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            dir.y = 1;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            dir.x = 1;
        }

        if(dir.x != 0 || dir.y != 0)
        {
            moveController.MoveInDir(dir);
            hasMoved = true;
        }
    }
}
