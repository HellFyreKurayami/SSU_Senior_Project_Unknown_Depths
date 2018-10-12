using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

///<summary>
///Map Maker Tester
///
///This class contains code references to both a tutorial located on
///the Unity site, and a tutorial found on LinkedIn Learning/Lynda by
///Sebstian Lague and Jesse Freeman Respectively
///
///https://unity3d.com/learn/tutorials/projects/procedural-cave-generation-tutorial/
///https://www.linkedin.com/learning/unity-5-2d-random-map-generation
///</summary>

[CustomEditor(typeof(MapMaker))]
public class MapMakerTester : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (MapMaker)target;

        if (GUILayout.Button("Generate Map"))
        {
            if (Application.isPlaying)
            {
                script.Create();
            }
        }

        if (GUILayout.Button("Create Player"))
        {
            if (Application.isPlaying)
            {
                script.createPlayer();
            }
        }
    }
}
