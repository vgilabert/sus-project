using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace TerrainGeneration
{
    public class PropsGenerator
    {
        public static PropsData GeneratePropsData(MeshData meshData, Vector2 position)
        {
            PropsData propsData = new PropsData();
            var vertices = meshData.vertices;
            
            Random Random = new Random();
            for (int i = 0; i < vertices.Length; i += Random.Next(10, 50))
            {
                Debug.Log(i);
                if (vertices[i].y > 1)
                {
                    propsData.AddBuilding(vertices[i] + new Vector3(position.x, 0, position.y));
                }
            }

            return propsData;
        }
    }

    public class PropsData
    {
        private List<Vector3> buildingsPosition;
        public List<Vector3> BuildingsPosition => buildingsPosition;

        public PropsData()
        {
            buildingsPosition = new List<Vector3>();
        }
        
        public void AddBuilding(Vector3 position)
        {
            buildingsPosition.Add(position);
        }
        
    }
}