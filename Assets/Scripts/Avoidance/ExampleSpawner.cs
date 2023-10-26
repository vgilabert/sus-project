﻿using UnityEngine;
using UnityEngine.AI;

namespace NavMeshAvoidance
{
    public sealed class ExampleSpawner : MonoBehaviour
    {
        [Header("Testing settings")]
        [SerializeField] GameObject agentPrefab;
        [SerializeField, Range(1,500)] int count = 16;
        
        [Header("Components links")]
        Ordering ordering;
        Avoidance avoidance;

        readonly IFormation spawnFormation = new SquareFormation();
        
        void Start()
        {
            //ordering.Avoidance = avoidance;
            ordering = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<Ordering>();
            avoidance = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<Avoidance>();

            var spawnPositions = spawnFormation.GetPositions(transform.position, count, 1);
            
            for (var i = 0; i < count; i++)
                DoSpawn(spawnPositions[i]);
        }

        void DoSpawn(Vector3 position)
        {
            var spawned = Instantiate(agentPrefab, position, Quaternion.identity);
            var agent = spawned.GetComponent<NavMeshAgent>();

            if(ordering != null)
                ordering.AddAgent(agent);
            if(avoidance != null)
                avoidance.AddAgent(agent);
        }
    }
}