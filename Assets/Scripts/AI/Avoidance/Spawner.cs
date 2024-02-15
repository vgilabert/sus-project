using System;
using NavMeshAvoidance;
using UnityEngine;
using UnityEngine.AI;

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
    
    public bool spawnOnAwake = false;
    public bool hasSpawned = false;

    readonly IFormation spawnFormation = new SquareFormation();
    public Vector3 spawnPosition;

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
        hasSpawned = true;
        for (var i = 0; i < count; i++)
        {
            var spawnPositions = spawnFormation.GetPositions(spawnPosition, count, 1);
            DoSpawn(spawnPositions[i]);
        }
    }

    Transform DoSpawn(Vector3 position)
    {
        var spawned = Instantiate(agentPrefab, position, Quaternion.identity, enemiesParent);
        var agent = spawned.GetComponent<NavMeshAgent>();

        if (ordering != null)
            ordering.AddAgent(agent);
        if (avoidance != null)
            avoidance.AddAgent(agent);
        if (finder)
            finder.AddAgent(spawned.transform);

        return spawned.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawnOnAwake || hasSpawned)
        {
            return;
        }
        if (other.CompareTag("Train"))
        {
            TriggerSpawn();
        }
    }
}