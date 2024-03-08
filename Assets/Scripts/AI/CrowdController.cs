 using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

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
    
    TransformAccessArray enemyTransforms;
    List<Enemy> enemyList = new List<Enemy>();
    List<IDamageable> targetList = new List<IDamageable>();

    NativeArray<float3> enemyPositions;
    NativeArray<float3> targetPositions;

    NativeArray<float3> NearestTargetPositions;
    NativeArray<int> NearestTargetIndex;

    void OnDestroy()
    {
        enemyPositions.Dispose();
        targetPositions.Dispose();

        NearestTargetPositions.Dispose();
        NearestTargetIndex.Dispose(); 
    }
    
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyPositions = new NativeArray<float3>(enemyList.Count, Allocator.Persistent);
        targetPositions = new NativeArray<float3>(targetList.Count, Allocator.Persistent);
        
        NearestTargetPositions = new NativeArray<float3>(enemyList.Count, Allocator.Persistent);
        NearestTargetIndex = new NativeArray<int>(enemyList.Count, Allocator.Persistent);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (enemyPositions.Length != enemyList.Count)
        {
            enemyPositions.Dispose();
            NearestTargetPositions.Dispose();
            NearestTargetIndex.Dispose();
            
            enemyPositions = new NativeArray<float3>(enemyList.Count, Allocator.Persistent);
            NearestTargetPositions = new NativeArray<float3>(enemyList.Count, Allocator.Persistent);
            NearestTargetIndex = new NativeArray<int>(enemyList.Count, Allocator.Persistent);
        }

        if(targetPositions.Length != targetList.Count)
        {
            targetPositions.Dispose();
            targetPositions = new NativeArray<float3>(targetList.Count, Allocator.Persistent);
        }
             
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyPositions[i] = enemyList[i].transform.position;
        }
        
        for (int i = 0; i < targetList.Count; i++)
        {
            targetPositions[i] = targetList[i].transform.position;
        }
        
        FindNearestJob findJob = new FindNearestJob
        {
            TargetPositions = targetPositions,
            SeekerPositions = enemyPositions,
            NearestTargetPositions = NearestTargetPositions,
            NearestTargetIndex = NearestTargetIndex
        };

        // Schedule() puts the job instance on the job queue.
        JobHandle findHandle = findJob.Schedule();

        // The Complete method will not return until the job represented by
        // the handle finishes execution. Effectively, the main thread waits
        // here until the job is done.

        findHandle.Complete();
        
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].SetNearestTargetPositionAndIndex(NearestTargetPositions[i], NearestTargetIndex[i]);
        }
    }

    public NativeArray<float3> GetEnemyPositions() => enemyPositions;
    public List<Enemy> GetEnemyList() => enemyList;
    
    public IDamageable GetTarget(int index) => targetList[index]; 
    
    public int GetEnemyCount() => enemyList.Count;

    public void AddAgent(Enemy agent)
    {
        enemyList.Add(agent);
    } 
    
    public void RemoveAgent(Enemy agent)
    {
        int index = enemyList.IndexOf(agent);
        enemyList.RemoveAt(index);
    } 
    
    public void AddTarget(IDamageable target)
    {
        targetList.Add(target);
    }
    
    public void RemoveTarget(IDamageable target)
    {
        int index = targetList.IndexOf(target);
        targetList.RemoveAt(index);
    }
}
