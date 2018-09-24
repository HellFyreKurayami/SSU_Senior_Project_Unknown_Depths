using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


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
    }
}
