using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//RNG Map Test Editor Class

//Most of this code is in refernece to a 2D Map
//Generation tutuorial found on LinkedIn Learning or
//Lynda.com. Tutorial by Jesse Freeman

//https://www.linkedin.com/learning/unity-5-2d-random-map-generation

[CustomEditor(typeof(RNGMapTest))]
public class RNGMapTestEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (RNGMapTest)target;

        if(GUILayout.Button("Generate Map"))
        {
            if (Application.isPlaying)
            {
                script.MakeMap();
            }
        }
        base.OnInspectorGUI();
    }
}
