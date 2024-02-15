using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dreamteck.Splines;
using PimDeWitte.UnityMainThreadDispatcher;
using TerrainGeneration;
using Unity.AI.Navigation;


public class EndlessTerrain : MonoBehaviour
{
	public const float maxViewDst = 150;
	public Transform viewer;
	public Material mapMaterial;

	public static Vector2 viewerPosition;
	static MapGenerator mapGenerator;
	int chunkSize;
	int chunksVisibleInViewDst;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new();
	List<TerrainChunk> terrainChunksVisibleLastUpdate = new ();
	
	static NavMeshSurface navMeshSurface;
	
	

	void Start() {
		mapGenerator = FindFirstObjectByType<MapGenerator> ();
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
		navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
	}

	void Update() {
		if (viewer)
			viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);
		UpdateVisibleChunks();
	}
	void UpdateVisibleChunks() {

		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
			terrainChunksVisibleLastUpdate [i].SetVisible (false);
		}
		terrainChunksVisibleLastUpdate.Clear ();
			
		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / chunkSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
					terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk ();
					if (terrainChunkDictionary [viewedChunkCoord].IsVisible ()) {
						terrainChunksVisibleLastUpdate.Add (terrainChunkDictionary [viewedChunkCoord]);
					}
				} 
				else
				{
					
					terrainChunkDictionary.Add(viewedChunkCoord,new TerrainChunk(viewedChunkCoord,chunkSize, transform, mapMaterial));

				}

			}
		}
	}

	public class TerrainChunk {

		GameObject meshObject;
		Vector2 position;
		Bounds bounds;

		MeshRenderer meshRenderer;
		MeshFilter meshFilter;
		MeshCollider meshCollider;
		
		public TerrainChunk(Vector2 coord, int size, Transform parent, Material material) {
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
			
			
			SetVisible(false);

			Task.Run(() =>
			{
				mapGenerator.RequestMapData(position, OnMapDataReceived);
			});
		}

		void OnMapDataReceived(MapGenerator.MapData mapData)
		{
			Task.Run(() =>
			{
				mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
			});
		}

		 void OnMeshDataReceived(MeshData meshData)
		 {
			 Task.Run(() =>
			 {
				 GenerateRoad(meshData, position, OnRoadGenerated); 
			 });
		 }

		 void OnRoadGenerated(MeshData meshData)
		 {
			 UnityMainThreadDispatcher.Instance().Enqueue(() =>
			 {
				 meshFilter.mesh = meshData.CreateMesh();
				 meshCollider.sharedMesh = meshFilter.mesh;
				 navMeshSurface.BuildNavMesh();
			 });
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

		public void UpdateTerrainChunk() {
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance (viewerPosition));
			bool visible = viewerDstFromNearestEdge <= maxViewDst;
			SetVisible (visible);
		}

		public void SetVisible(bool visible) {
			meshObject.SetActive (visible);
		}

		public bool IsVisible() {
			return meshObject.activeSelf;
		}

	}
}
