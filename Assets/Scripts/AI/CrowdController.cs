using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public  class CrowdController :MonoBehaviour
{
    private static CrowdController instance;
    public static CrowdController Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("Crowd controller not found!");
            return instance;
        }
    }

    List<Enemy> enemyList = new List<Enemy>();
    NativeArray<float3> enemyPositions;
    void OnDestroy()
    {
        enemyPositions.Dispose();
    }
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyPositions = new NativeArray<float3>(enemyList.Count, Allocator.Persistent);
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyPositions.Length != enemyList.Count)
        {
            enemyPositions.Dispose();
            enemyPositions = new NativeArray<float3>(enemyList.Count, Allocator.Persistent);
        }

        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyPositions[i] = enemyList[i].transform.position;
        }
    }

    public NativeArray<float3> GetEnemyPositions() => enemyPositions;
    public List<Enemy> GetEnemyList() => enemyList;
    public void AddAgent(Enemy agent) => enemyList.Add(agent);
    public void RemoveAgent(Enemy agent) => enemyList.Remove(agent);
}
