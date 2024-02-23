using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using TerrainGeneration;
using Train;
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
		viewer = FindFirstObjectByType<Engine>().transform;
	}

	void Update() {
		if (viewer)
		{
			viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);
		}
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
			meshObject.layer = LayerMask.NameToLayer("Terrain");
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
				mapGenerator.RequestMeshData(mapData, position, OnMeshDataReceived);
			});
		}
		
		void OnMeshDataReceived(MeshData meshData)
		{
			Task.Run(() =>
			{
				mapGenerator.RequestPropsData(meshData, position, OnPropsDataReceived);
			});
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
				int buildingsCount = mapGenerator.buildings.Length;
				int random = Random.Range(0, buildingsCount-1);
				Instantiate(mapGenerator.buildings[random], buildingPosition, Quaternion.identity, meshObject.transform);
			}
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
