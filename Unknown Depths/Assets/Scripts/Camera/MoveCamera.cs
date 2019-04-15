using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///Move Camera Class
///
///Most of this code is in refernece to
///tutuorials found on LinkedIn Learning or
///Lynda.com. Tutoriald by Jesse Freeman
///
///Will be updated to be attached to the player later
///
///https://www.linkedin.com/learning/unity-5-2d-random-map-generation
///https://www.linkedin.com/learning/unity-5-2d-movement-in-an-rpg-game
/// </summary>

public class MoveCamera : MonoBehaviour {

    public float speed = 4f;
    public GameObject target;

    private void FixedUpdate()
    {
        if(target != null)
        {
            var pos = target.transform.position;
            pos.z = Camera.main.transform.position.z;

            Camera.main.transform.position = pos;
        }
    }
}
