using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dreamteck.Splines;
using NUnit.Framework;
using Spline;
using TerrainGeneration;
using Train;
using TreeEditor;
using Unity.AI.Navigation;
using Random = UnityEngine.Random;


public class EndlessTerrain : MonoBehaviour
{
    public Material mapMaterial;

    static MapGenerator mapGenerator;
    int chunkSize;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new();

    static NavMeshSurface navMeshSurface;

    static Transform propsParent;
    
    private Vector2 currentChunkCoord;

    private SplineComputer splineA;
    private SplineComputer splineB;

    public static Action<SplineComputer> OnRailsCreated = delegate { };
    
    enum Direction
    {
        North,
        East,
        South
    }
    
    void Start()
    {
        mapGenerator = FindFirstObjectByType<MapGenerator>();
        chunkSize = MapGenerator.mapChunkSize - 1;
        navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
        propsParent = new GameObject("Props").transform;
        propsParent.parent = transform;
        var nextCoord = GetNextCoord(Vector3.zero);
        splineB = GenerateSpline(Vector3.zero, ChunkCoordToPosition(nextCoord));
        
        Node node = CreateNode(splineB, splineB.GetPoints().Length - 1);
        node.AddConnection(splineB, splineB.GetPoints().Length - 1);
        
        List<SplineComputer> splines = new()
        {
            splineB
        };
        OnRailsCreated(splineB);
        GenerateChunk(currentChunkCoord, splines);
        currentChunkCoord = nextCoord;
    }
    
    void Update()
    {
        GeneratePath();
    }
    
    Vector2 GetNextCoord(Vector2 startPos)
    {
        Vector2 pos = Vector2.zero;
        var direction = (Direction) Random.Range(0, 2);
        switch (direction)
        {
            case Direction.North:
                pos = startPos + Vector2.up;
                break;
            case Direction.East:
                pos = startPos + Vector2.right;
                break;
            case Direction.South:
                pos = startPos + Vector2.down;
                break;
        }

        return pos;
    }

    void GeneratePath()
    {
        /*if (CurrentProgression() > 0.9f)
        {
            GenerateNextSpline();
        }*/
        if (Input.GetKeyDown(KeyCode.P))
        {
            var nextCoord = GetNextCoord(currentChunkCoord);
            splineA = splineB;
            splineB = GenerateSpline(ChunkCoordToPosition(currentChunkCoord),
                ChunkCoordToPosition(nextCoord));
            
            Node nodeA = splineA.GetComponentInChildren<Node>();
            nodeA.AddConnection(splineB, 0);
            
            Node nodeTest = CreateNode(splineA, 3);
            
            Node nodeB = CreateNode(splineB, splineB.GetPoints().Length - 1);
            nodeB.AddConnection(splineB, splineB.GetPoints().Length - 1);
            
            List<SplineComputer> splines = new()
            {
                splineA,
                splineB
            };
            GenerateChunk(currentChunkCoord, splines);
            currentChunkCoord = nextCoord;
        }
    }
    
    SplineComputer GenerateSpline(Vector3 startPos, Vector3 endPos)
    {
        return mapGenerator.splineGenerator.RandomSplineBetweenPoints(startPos, endPos);
    }

    void GenerateChunk(Vector2 coord, List<SplineComputer> splines)
    {
        var chunk = new TerrainChunk(coord, chunkSize, transform, mapMaterial, splines);
        terrainChunkDictionary.Add(coord, chunk);
        chunk.SetVisible(true);
    }
    
    Node CreateNode(SplineComputer spline, int index)
    {
        Node node = new GameObject("Node").AddComponent<Node>();
        node.transform.position = spline.GetPointPosition(index);
        node.transform.parent = spline.transform;
        return node;
    }

    Vector3 ChunkCoordToPosition(Vector2 coord)
    {
        return new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);
    }
    
    
    
    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;
        List<SplineComputer> splines;

        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material, List<SplineComputer> splines)
        {
            position = coord * size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            this.splines = splines;
            
            meshObject = new GameObject("Terrain Chunk");
            meshObject.layer = LayerMask.NameToLayer("Terrain");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            
            SetVisible(true);

            Task.Run(() => { mapGenerator.RequestMapData(position, OnMapDataReceived); });
        }

        void OnMapDataReceived(MapGenerator.MapData mapData)
        {
            var roadPath = mapGenerator.splineGenerator.SplinesToPoints(splines);
            Task.Run(() => { mapGenerator.RequestMeshData(mapData, position, roadPath, OnMeshDataReceived); });
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            Task.Run(() => { mapGenerator.RequestPropsData(meshData, position, OnPropsDataReceived); });
            meshFilter.mesh = meshData.CreateMesh();
            meshCollider.sharedMesh = meshFilter.mesh;
            meshRenderer.material = mapGenerator.terrainMaterial;
            meshRenderer.material.mainTextureScale = mapGenerator.textureScale;
            LayerMask mask = LayerMask.GetMask("Terrain");
            navMeshSurface.layerMask = mask;
            navMeshSurface.BuildNavMesh();
        }

        void OnPropsDataReceived(PropsData propsData)
        {
            foreach (var buildingPosition in propsData.BuildingsPosition)
            {
                int buildingsCount = mapGenerator.buildingPrefabs.Length;
                int random = Random.Range(0, buildingsCount - 1);
                Instantiate(mapGenerator.buildingPrefabs[random], buildingPosition, Quaternion.identity, propsParent);
            }

            foreach (var lootBoxPosition in propsData.lootBoxPosition)
            {
                Instantiate(mapGenerator.lootBoxPrefab, lootBoxPosition, Quaternion.identity, propsParent);
            }
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}