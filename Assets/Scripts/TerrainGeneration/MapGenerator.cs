﻿using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
 using System.Threading.Tasks;
 using AI;
 using Dreamteck.Splines;
 using TerrainGeneration;
 using UnityEditor.AssetImporters;
 using MeshGenerator = TerrainGeneration.MeshGenerator;

 public class MapGenerator : MonoBehaviour
 {
	 public const int mapChunkSize = 241;
	 [Range(0, 6)] public int levelOfDetail;
	 public float noiseScale;

	 public int octaves;
	 [Range(0, 1)] public float persistance;
	 public float lacunarity;

	 public int seed;
	 public Vector2 offset;

	 public float meshHeightMultiplier;
	 public AnimationCurve meshHeightCurve;
	 
	 public Material terrainMaterial;
	 public Vector2 textureScale;

	 public bool autoUpdate;
	 
	 [Header("Path Settings")]
	 public SplineComputer spline;

	 public int pathWidth;
	 public AnimationCurve roadSlopeCurve;

	 Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new ();
	 Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new ();

	 private void Start()
	 {
		 SpawnManager.Instance.GenerateSpawners(spline);
	 }

	 public void DrawMapInEditor()
	 {
		 MapData mapData = GenerateMapData(Vector2.zero);

		 MapDisplay display = FindFirstObjectByType<MapDisplay>();
		 display.DrawMesh(
			 MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve,
				 levelOfDetail));
	 }

	 public void RequestMapData(Vector2 centre, Action<MapData> callback)
	 {
		 Task.Run(() =>
		 {
			 MapDataThread(centre, callback);
		 });
	 }

	 void MapDataThread(Vector2 centre, Action<MapData> callback)
	 {
		 MapData mapData = GenerateMapData(centre);
		 lock (mapDataThreadInfoQueue)
		 {
			 mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
		 }
	 }

	 public void RequestMeshData(MapData mapData, Action<MeshData> callback)
	 {
		 Task.Run(() =>
		 {
			 MeshDataThread(mapData, callback);
		 });
	 }

	 void MeshDataThread(MapData mapData, Action<MeshData> callback)
	 {
		 MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve,
			 levelOfDetail);
		 lock (meshDataThreadInfoQueue)
		 {
			 meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
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