using UnityEngine;
using System;
using System.Collections.Generic;
 using System.Threading.Tasks;
 using TerrainGeneration;
 using UnityEngine.Serialization;
 using MeshGenerator = TerrainGeneration.MeshGenerator;

public class MapGenerator : MonoBehaviour
{
	public const int mapChunkSize = 241;
	[UnityEngine.Range(0, 6)] public int levelOfDetail;
	public float noiseScale;

	public int octaves;
	[UnityEngine.Range(0, 1)] public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

	public Material terrainMaterial;
	public Vector2 textureScale;

	public bool autoUpdate;

	[Header("Props Settings")] public GameObject[] buildingPrefabs;
	public GameObject lootBoxPrefab;

	[FormerlySerializedAs("SplineGenerator")] public SplineGenerator splineGenerator;

	public int pathWidth;
	public AnimationCurve roadSlopeCurve;

	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new();
	Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new();
	Queue<MapThreadInfo<PropsData>> propsDataThreadInfoQueue = new();

	public void DrawMapInEditor()
	{
		MapData mapData = GenerateMapData(Vector2.zero);
		
		MapDisplay display = FindFirstObjectByType<MapDisplay>();
		display.DrawMesh(
			MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve,
				levelOfDetail, Vector3.zero, null, pathWidth, roadSlopeCurve));
	}

	public void RequestMapData(Vector2 centre, Action<MapData> callback)
	{
		Task.Run(() => { MapDataThread(centre, callback); });
	}

	void MapDataThread(Vector2 centre, Action<MapData> callback)
	{
		MapData mapData = GenerateMapData(centre);
		lock (mapDataThreadInfoQueue)
		{
			mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
		}
	}

	public void RequestMeshData(MapData mapData, Vector2 position, List<Vector3> roadPath, Action<MeshData> callback)
	{
		Task.Run(() => { MeshDataThread(mapData, position, roadPath, callback); });
	}

	void MeshDataThread(MapData mapData, Vector2 position, List<Vector3> roadPath, Action<MeshData> callback)
	{
		MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve,
			levelOfDetail, position, roadPath, pathWidth, roadSlopeCurve);
		lock (meshDataThreadInfoQueue)
		{
			meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
		}
	}

	public void RequestPropsData(MeshData meshData, Vector2 position, Action<PropsData> callback)
	{
		Task.Run(() => { PropsDataThread(meshData, position, callback); });
	}

	void PropsDataThread(MeshData meshData, Vector2 position, Action<PropsData> callback)
	{
		PropsData propsData = PropsGenerator.GeneratePropsData(meshData, position);
		lock (propsDataThreadInfoQueue)
		{
			propsDataThreadInfoQueue.Enqueue(new MapThreadInfo<PropsData>(callback, propsData));
		}
	}

	void Update()
	{
		if (mapDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
			{
				MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}

		if (meshDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
			{
				MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}

		if (propsDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < propsDataThreadInfoQueue.Count; i++)
			{
				MapThreadInfo<PropsData> threadInfo = propsDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}
	}

	MapData GenerateMapData(Vector2 centre)
	{
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance,
			lacunarity, offset + centre, Noise.NormalizeMode.Global);

		return new MapData(noiseMap);
	}

	void OnValidate()
	{
		if (lacunarity < 1)
		{
			lacunarity = 1;
		}

		if (octaves < 0)
		{
			octaves = 0;
		}
	}

	struct MapThreadInfo<T>
	{
		public readonly Action<T> callback;
		public readonly T parameter;

		public MapThreadInfo(Action<T> callback, T parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}
	}

	public struct MapData
	{
		public readonly float[,] heightMap;

		public MapData(float[,] heightMap)
		{
			this.heightMap = heightMap;
		}
	}
}