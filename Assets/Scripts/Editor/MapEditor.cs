using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapGenerator_Hugal))]
public class MapEditor : Editor
{

    public override void OnInspectorGUI()
    {

        MapGenerator_Hugal map = target as MapGenerator_Hugal;

        if (DrawDefaultInspector())
        {
            map.GenerateMap();
        }

        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }


    }

}