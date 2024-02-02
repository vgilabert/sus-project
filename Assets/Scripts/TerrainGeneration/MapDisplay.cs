using TerrainGeneration;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public MeshFilter meshFilter;
    
    public void DrawMesh(MeshData meshData) {
        meshFilter.sharedMesh = meshData.CreateMesh ();
    }

}