using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Move Camera Class

//Most of this code is in refernece to a 2D Map
//Generation tutuorial found on LinkedIn Learning or
//Lynda.com. Tutorial by Jesse Freeman

//Will be updated to be attached to the player later

//https://www.linkedin.com/learning/unity-5-2d-random-map-generation

public class MoveCamera : MonoBehaviour {

    public float speed = 4f;

    private Vector3 startPos;
    private bool moving;

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            startPos = Input.mousePosition;
            moving = true;
        }

        if(Input.GetMouseButtonUp(1) && moving)
        {
            moving = false;
        }

        if (moving)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - startPos);
            Vector3 move = new Vector3(pos.x * speed, pos.y * speed, 0);
            transform.Translate(move, Space.Self);
        }
    }
}
