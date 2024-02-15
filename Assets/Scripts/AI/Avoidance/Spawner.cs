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
    
    public bool spawnOnAwake = false;

    readonly IFormation spawnFormation = new SquareFormation();
    public Vector3 spawnPosition;

    void Start()
    {
        //ordering.Avoidance = avoidance;
        ordering = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<Ordering>();
        avoidance = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<Avoidance>();
        finder = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<FindNearest>();
        GameObject[] t = GameObject.FindGameObjectsWithTag("Player");
        if (spawnOnAwake)
            TriggerSpawn();
    }
    
    public void TriggerSpawn()
    {
        for (var i = 0; i < count; i++)
        {
            var spawnPositions = spawnFormation.GetPositions(spawnPosition, count, 1);
            DoSpawn(spawnPositions[i]);
        }
    }

    Transform DoSpawn(Vector3 position)
    {
        var spawned = Instantiate(agentPrefab, position, Quaternion.identity);
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
        if (other.CompareTag("Train"))
        {
            TriggerSpawn();
        }
    }
}