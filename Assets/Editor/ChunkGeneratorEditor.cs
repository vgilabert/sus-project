using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class ChunkGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator terrainGenerator = (MapGenerator) target;

        if (DrawDefaultInspector())
        {
            if (terrainGenerator.autoUpdate)
            {
                terrainGenerator.DrawMapInEditor ();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            terrainGenerator.DrawMapInEditor ();
        }
    }
}
