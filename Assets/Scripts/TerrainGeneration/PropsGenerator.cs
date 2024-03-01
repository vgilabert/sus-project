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
            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].y > 1)
                {
                    if (Random.Next(0, 100) < 5)
                        propsData.AddBuilding(vertices[i] + new Vector3(position.x, 0, position.y));
                }
                else if (vertices[i].y == 0)
                {
                    if (Random.Next(0, 100) < 2)
                        propsData.AddLootBox(vertices[i] + new Vector3(position.x, 0, position.y));
                }
            }

            return propsData;
        }
    }

    public class PropsData
    {
        private List<Vector3> buildingsPosition;
        public List<Vector3> BuildingsPosition => buildingsPosition;
        
        public List<Vector3> lootBoxPosition;
        public List<Vector3> LootBoxPosition => lootBoxPosition;

        public PropsData()
        {
            buildingsPosition = new List<Vector3>();
            lootBoxPosition = new List<Vector3>();
        }
        
        public void AddBuilding(Vector3 position)
        {
            buildingsPosition.Add(position);
        }
        
        public void AddLootBox(Vector3 position)
        {
            lootBoxPosition.Add(position);
        }
        
    }
}