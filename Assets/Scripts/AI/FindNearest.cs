using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;


public class FindNearest : MonoBehaviour
{
    // The size of our arrays does not need to vary, so rather than create
    // new arrays every field, we'll create the arrays in Awake() and store them
    // in these fields.
    NativeArray<float3> TargetPositions;
    NativeArray<float3> SeekerPositions;
    NativeArray<float3> NearestTargetPositions;

    List<Transform> Targets = new List<Transform>();
    List<Transform> Agents = new List<Transform>();

    public void Start()
    {
        ExampleSpawner spawner = Object.FindObjectOfType<ExampleSpawner>();

        GameObject[] t = GameObject.FindGameObjectsWithTag ("Player");
        for (int i = 0; i < t.Length; i++) { Targets.Add(t[i].transform); }
        // We use the Persistent allocator because these arrays must
        // exist for the run of the program.
        TargetPositions = new NativeArray<float3>(Targets.Count, Allocator.Persistent);
        SeekerPositions = new NativeArray<float3>(Agents.Count, Allocator.Persistent);
        NearestTargetPositions = new NativeArray<float3>(Agents.Count, Allocator.Persistent);
    }

    // We are responsible for disposing of our allocations
    // when we no longer need them.
    public void OnDestroy()
    {
        TargetPositions.Dispose();
        SeekerPositions.Dispose();
        NearestTargetPositions.Dispose();
    }


    public void Update()
    {
        // Copy every target transform to a NativeArray.
        for (int i = 0; i < TargetPositions.Length; i++)
        {
            // Vector3 is implicitly converted to float3
            TargetPositions[i] = Targets[i].localPosition;
        }


        // Copy every seeker transform to a NativeArray.
        for (int i = 0; i < SeekerPositions.Length; i++)
        {
            // Vector3 is implicitly converted to float3
            SeekerPositions[i] = Agents[i].localPosition;

        }

        // To schedule a job, we first need to create an instance and populate its fields.
        FindNearestJob findJob = new FindNearestJob
        {
            TargetPositions = TargetPositions,
            SeekerPositions = SeekerPositions,
            NearestTargetPositions = NearestTargetPositions,
        };

        // Schedule() puts the job instance on the job queue.
        JobHandle findHandle = findJob.Schedule();

        // The Complete method will not return until the job represented by
        // the handle finishes execution. Effectively, the main thread waits
        // here until the job is done.

        findHandle.Complete();

        // Draw a debug line from each seeker to its nearest target.
        for (int i = 0; i < SeekerPositions.Length; i++)
        {
            // float3 is implicitly converted to Vector3
            Debug.DrawLine(SeekerPositions[i], NearestTargetPositions[i]);
        }
    }

    public void AddAgent(Transform agent)
    {
        Agents.Add(agent);
        SeekerPositions = new NativeArray<float3>(Agents.Count, Allocator.Persistent);
    }

    public void RemoveAgent(Transform agent)
    {
        Agents.Remove(agent);
        SeekerPositions = new NativeArray<float3>(Agents.Count, Allocator.Persistent);
    }

    public void AddTarget(Transform target) => Targets.Add(target);
    public void RemoveTarget(Transform target) => Targets.Remove(target);
}

