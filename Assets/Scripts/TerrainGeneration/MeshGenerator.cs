using System.Collections.Generic;
using UnityEngine;

namespace TerrainGeneration
{
	public static class MeshGenerator {

		public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve meshHeightCurve, int levelOfDetail, Vector2 position, List<Vector3> roadPath = null, int pathWidth = 0, AnimationCurve roadSlopeCurve = null) {
			
			AnimationCurve heightCurve = new AnimationCurve (meshHeightCurve.keys);

			int width = heightMap.GetLength (0);
			int height = heightMap.GetLength (1);
			float topLeftX = (width - 1) / -2f;
			float topLeftZ = (height - 1) / 2f;

			int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail * 2;
			int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
			
			Vector3 positionV3 = new Vector3(position.x,0,position.y);

			MeshData meshData = new MeshData (verticesPerLine, verticesPerLine);
			int vertexIndex = 0;

			for (int y = 0; y < height; y += meshSimplificationIncrement) {
				for (int x = 0; x < width; x += meshSimplificationIncrement) {
					Vector3 vertexPosition = new Vector3 (topLeftX + x, heightCurve.Evaluate (heightMap [x, y]) * heightMultiplier, topLeftZ - y);
					Vector3 worldVertexPosition = vertexPosition + positionV3;
					
					if (roadPath != null && roadSlopeCurve != null) {
						foreach (Vector3 roadPoint in roadPath) {
							float distanceToRoad = Vector3.Distance(new Vector3(worldVertexPosition.x, 0, worldVertexPosition.z), new Vector3(roadPoint.x, 0, roadPoint.z));
							if (distanceToRoad < pathWidth) {
								float slope = roadSlopeCurve.Evaluate(distanceToRoad / pathWidth) * vertexPosition.y;
								vertexPosition.y = 0 + slope;
							}
						}
					}

					meshData.vertices[vertexIndex] = vertexPosition;
					meshData.uvs [vertexIndex] = new Vector2 (x / (float)width, y / (float)height);

					if (x < width - 1 && y < height - 1) {
						meshData.AddTriangle (vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
						meshData.AddTriangle (vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
					}

					vertexIndex++;
				}
			}

			return meshData;

		}
	}

	public class MeshData {
		public Vector3[] vertices;
		public int[] triangles;
		public Vector2[] uvs;

		int triangleIndex;

		public MeshData(int meshWidth, int meshHeight) {
			vertices = new Vector3[meshWidth * meshHeight];
			uvs = new Vector2[meshWidth * meshHeight];
			triangles = new int[(meshWidth-1)*(meshHeight-1)*6];
		}

		public void AddTriangle(int a, int b, int c) {
			triangles [triangleIndex] = a;
			triangles [triangleIndex + 1] = b;
			triangles [triangleIndex + 2] = c;
			triangleIndex += 3;
		}

		public Mesh CreateMesh() {
			Mesh mesh = new Mesh ();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;
			mesh.RecalculateNormals ();
			return mesh;
		}

	}
}