using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dreamteck.Splines;
using PimDeWitte.UnityMainThreadDispatcher;
using Unity.AI.Navigation;
using UnityEngine;

namespace TerrainGeneration
{
	public class TerrainChunk
	{
		private MapGenerator mapGenerator;

		GameObject meshObject;
		Vector2 position;
		Bounds bounds;

		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		MeshCollider meshCollider;
		GameObject spawnZone;
		NavMeshSurface navMesh;
		private Vector3 viewerPosition;
		private float maxViewDst;
	
		public TerrainChunk(MapGenerator mapGenerator, Vector2 coord, int size, Transform parent, Material material, NavMeshSurface navMesh, GameObject spawnZone, Vector3 viewerPosition, float maxViewDst) {
			position = coord * size;
			bounds = new Bounds(position,Vector2.one * size);
			Vector3 positionV3 = new Vector3(position.x,0,position.y);

			meshObject = new GameObject("Terrain Chunk");
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshRenderer.material = material;
			meshFilter = meshObject.AddComponent<MeshFilter>();
			meshCollider = meshObject.AddComponent<MeshCollider>();

			meshObject.transform.position = positionV3;
			meshObject.transform.parent = parent;
			this.navMesh = navMesh;
			this.spawnZone = spawnZone;
			this.viewerPosition = viewerPosition;
			this.maxViewDst = maxViewDst;
		
		
			SetVisible(false);

			Task.Run(() =>
			{
				mapGenerator.RequestMapData(position, OnMapDataReceived);
				Debug.Log("TerrainChunk");
			});
		}

		void OnMapDataReceived(MapGenerator.MapData mapData)
		{
			Task.Run(() =>
			{
				mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
				Debug.Log("OnMapDataReceived");
			});
		}

		void OnMeshDataReceived(MeshData meshData)
		{
			Task.Run(() =>
			{
				GenerateRoad(meshData, position, OnRoadGenerated); 
				Debug.Log("OnMeshDataReceived");
			});
		}

		void OnRoadGenerated(MeshData meshData)
		{
			UnityMainThreadDispatcher.Instance().Enqueue(() =>
			{
				meshFilter.mesh = meshData.CreateMesh();
				meshCollider.sharedMesh = meshFilter.mesh;
				navMesh.BuildNavMesh();
				GenerateSpawners();
				Debug.Log("OnRoadGenerated");
			});
		}

		public void GenerateSpawners()
		{
			List<Vector3> points = SplineToWorldPoints(mapGenerator.spline);
			points = GetRandomPointsOnSpline(0.1, 0.2);

			List<Vector3> GetRandomPointsOnSpline(double distanceMin, double distanceMax)
			{
				var random = new System.Random();
				double positionOnSpline = 0;
				var randompoints = new List<Vector3>();
				while (positionOnSpline < 1)
				{
					var distance = random.NextDouble() * (distanceMax - distanceMin) + distanceMin;
					positionOnSpline += distance;
					points.Add(mapGenerator.spline.EvaluatePosition(positionOnSpline));
				}

				return randompoints;
			}

			foreach (var p in points)
			{
				GameObject.Instantiate(spawnZone, p, Quaternion.identity);
			}

			Debug.Log("GenerateSpawners");
		}

		public void GenerateRoad(MeshData meshData, Vector2 centre, Action<MeshData> onRoadGenerated)
		{
			List<Vector3> points = SplineToWorldPoints(mapGenerator.spline);
			for (int i = 0; i < points.Count; i++)
			{
				Vector3 point = points[i];
				for (int j = 0; j < meshData.vertices.Length; j++)
				{
					Vector3 vertex = meshData.vertices[j];
					var distance = Vector3.Distance(point, new Vector3(vertex.x, 0, vertex.z) + new Vector3(centre.x, 0, centre.y));
					if (distance < mapGenerator.pathWidth)
					{
						var newHeight = 0 + mapGenerator.roadSlopeCurve.Evaluate(distance / mapGenerator.pathWidth) * vertex.y;
						meshData.vertices[j] = new Vector3(vertex.x, newHeight, vertex.z);
					}
				}
			}
			onRoadGenerated?.Invoke(meshData);
			Debug.Log("GenerateRoad");
		}

		private List<Vector3> SplineToWorldPoints(SplineComputer spline)
		{
			List<Vector3> points = new List<Vector3>();
			for (int i = 0; i < 100; i += 2)
			{
				var percent = i / 100f;
				points.Add(spline.EvaluatePosition(percent));
			}
			Debug.Log("SplineToWorldPoints");
			return points;
		}

		public void UpdateTerrainChunk() {
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance (viewerPosition));
			bool visible = viewerDstFromNearestEdge <= maxViewDst;
			SetVisible (visible);
			Debug.Log("UpdateTerrainChunk");
		}

		public void SetVisible(bool visible) {
			meshObject.SetActive (visible);
		}

		public bool IsVisible() {
			return meshObject.activeSelf;
		}

	}
}