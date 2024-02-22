using System;
using NavMeshAvoidance;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public sealed class Spawner : MonoBehaviour
{
    [Header("Testing settings")]
    public GameObject agentPrefab;
    public int count = 16;

    [Header("Components links")]
    Ordering ordering;
    Avoidance avoidance;
    FindNearest finder;
    
    Transform enemiesParent;
    
    public bool spawnOnAwake;

    readonly IFormation spawnFormation = new SquareFormation();

    void Start()
    {
        enemiesParent = GameObject.Find("Enemies").transform;
        //ordering.Avoidance = avoidance;
        ordering = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<Ordering>();
        avoidance = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<Avoidance>();
        finder = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<FindNearest>();
        if (spawnOnAwake)
            TriggerSpawn();
    }
    
    public void TriggerSpawn()
    {
        for (var i = 0; i < count; i++)
        {
            var spawnPositions = spawnFormation.GetPositions(transform.position, count, 1);
            DoSpawn(spawnPositions[i]);
        }
    }
    
    // Do the same as trigger spawn but delay the spawns over multiple frames
    public void TriggerSpawnDelayed(float delay)
    {
        StartCoroutine(SpawnDelayed(delay));
    }
    
    System.Collections.IEnumerator SpawnDelayed(float delay)
    {
        for (var i = 0; i < count; i++)
        {
            var spawnPositions = spawnFormation.GetPositions(transform.position, count, 1);
            DoSpawn(spawnPositions[i]);
            yield return new WaitForSeconds(delay);
        }
    }

    Transform DoSpawn(Vector3 position)
    {
        var spawned = Instantiate(agentPrefab, position, Quaternion.identity, enemiesParent);
        var agent = spawned.GetComponent<NavMeshAgent>();

        // check if the enemy is on a navmesh
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(position, out var hit, 100, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError("No navmesh found");
                //Destroy(spawned);
                return null;
            }
        }
        if (ordering != null)
            ordering.AddAgent(agent);
        if (avoidance != null)
            avoidance.AddAgent(agent);
        if (finder)
            finder.AddAgent(spawned.transform);

        return spawned.transform;
    }
}