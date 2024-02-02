using System.Collections.Generic;
using Dreamteck.Splines;
using NUnit.Framework;
using TerrainGeneration;
using UnityEngine;
using MeshGenerator = TerrainGeneration.MeshGenerator;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    
    const int mapChunkSize = 241;
    [UnityEngine.Range(0,6)]
    public int levelOfDetail;
    public float noiseScale;

    public int octaves;
    [UnityEngine.Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    [Header("Path Settings")]
    public SplineComputer spline;
    public int pathWidth;
    public AnimationCurve roadSlopeCurve;
    
    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        
        MapDisplay display = FindObjectOfType<MapDisplay>();
        MeshData meshData =
            MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail);
        
        GenerateRoad(meshData);
        
        display.DrawMesh(meshData);
    }
    
    private void GenerateRoad(MeshData meshData)
    {
        List<Vector3> points = SplineToWorldPoints(spline);
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[i];
            for (int j = 0; j < meshData.vertices.Length; j++)
            {
                Vector3 vertex = meshData.vertices[j];
                var distance = Vector3.Distance(point, vertex);
                if (distance < pathWidth)
                {
                    var newHeight = 0 + roadSlopeCurve.Evaluate(distance / pathWidth) * vertex.y;
                    meshData.vertices[j] = new Vector3(vertex.x, newHeight, vertex.z);
                }
            }
        }
    }
    
    private List<Vector3> SplineToWorldPoints(SplineComputer spline)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < 100; i += 2)
        {
            var percent = i / 100f;
            points.Add(spline.EvaluatePosition(percent));
        }
        return points;
    }

    void OnValidate() {
        if (lacunarity < 1) {
            lacunarity = 1;
        }
        if (octaves < 0) {
            octaves = 0;
        }
    }
}
