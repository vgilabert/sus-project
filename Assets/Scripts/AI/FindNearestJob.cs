using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

// We'll use Unity.Mathematics.float3 instead of Vector3,
// and we'll use Unity.Mathematics.math.distancesq instead of Vector3.sqrMagnitude.
using Unity.Mathematics;
using UnityEngine;


// Include the BurstCompile attribute to Burst compile the job.

[BurstCompile]
    public struct FindNearestJob : IJob
    {
        // All of the data which a job will access should 
        // be included in its fields. In this case, the job needs
        // three arrays of float3.

        // Array and collection fields that are only read in
        // the job should be marked with the ReadOnly attribute.
        // Although not strictly necessary in this case, marking data  
        // as ReadOnly may allow the job scheduler to safely run 
        // more jobs concurrently with each other.
        // (See the "Intro to jobs" for more detail.)

        [ReadOnly] public NativeArray<float3> TargetPositions;
        [ReadOnly] public NativeArray<float3> SeekerPositions;

        // For SeekerPositions[i], we will assign the nearest 
        // target position to NearestTargetPositions[i].
        public NativeArray<float3> NearestTargetPositions;
        public NativeArray<int> NearestTargetIndex;

        // 'Execute' is the only method of the IJob interface.
        // When a worker thread executes the job, it calls this method.
        public void Execute()
        {
            // Compute the square distance from each seeker to every target.
            for (int i = 0; i < SeekerPositions.Length; i++)
            {
                float3 seekerPos = SeekerPositions[i];
                float nearestDistSq = float.MaxValue;
                for (int j = 0; j < TargetPositions.Length; j++)
                {
                    float3 targetPos = TargetPositions[j];
                    float distSq = math.distancesq(seekerPos, targetPos);
                    if (distSq < nearestDistSq)
                    {
                        nearestDistSq = distSq;
                        NearestTargetPositions[i] = targetPos;
                        NearestTargetIndex[i] = j;
                    }
                }
            }
        }
    }

[BurstCompile]
public struct FindClosestTarget : IJob
{
    // All of the data which a job will access should 
    // be included in its fields. In this case, the job needs
    // three arrays of float3.

    // Array and collection fields that are only read in
    // the job should be marked with the ReadOnly attribute.
    // Although not strictly necessary in this case, marking data  
    // as ReadOnly may allow the job scheduler to safely run 
    // more jobs concurrently with each other.
    // (See the "Intro to jobs" for more detail.)

    [ReadOnly] public NativeArray<float3> TargetPositions;
    [ReadOnly] public float3 SeekerPosition;

    public NativeArray<int> NearestTargetIndex;
    public float maxDistance;
    public float minDistance;
    
    public void Execute()
    {
        // Compute the distance from each seeker to every target.
        float nearestDist = float.MaxValue;
        NearestTargetIndex[0] = -int.MaxValue;
        for (int j = 0; j < TargetPositions.Length; j++)
        {
            float3 targetPos = TargetPositions[j];
            // Get the distance from the seeker to the target.
            float dist = math.distance(SeekerPosition, targetPos);
                
            if (dist < nearestDist && dist < maxDistance && dist > minDistance)
            {
                nearestDist = dist;
                NearestTargetIndex[0] = j;
            }
        }
    }
}

[BurstCompile]
public struct ExplosionJob : IJob
{
    [ReadOnly] public float3 explosionPosition;
    [ReadOnly] public float explosionRadius;
    [ReadOnly] public NativeArray<float3> TargetPositions;
    public NativeArray<int> targetIndex;

    public void Execute()
    {
        for (int i = 0; i < TargetPositions.Length; i++)
        {
            float3 enemyPos = TargetPositions[i];
            float distSq = math.distancesq(explosionPosition, enemyPos);
            if (distSq < explosionRadius * explosionRadius)
            {
                targetIndex[i] = i;
            }
        }
    }
}

[BurstCompile]
public struct AvoidanceJob : IJob
{
    [ReadOnly] public NativeArray<float3> AgentPositions;
    public NativeArray<Vector3> AvoidanceVectors;
    public float maxAvoidanceDistance;
    public float strength;

    public void Execute()
    {
        var agentsTotal = AgentPositions.Length;
        for (var q = 0; q < agentsTotal; q++)
        {
            var agentAPos = AgentPositions[q];

            var avoidanceVector = float3.zero;

            for (var w = 0; w < agentsTotal; w++)
            {
                var agentBPos = AgentPositions[w];

                var direction = agentAPos - agentBPos;
                var sqrDistance = math.distancesq(agentAPos, agentBPos);

                var weakness = sqrDistance / (maxAvoidanceDistance * maxAvoidanceDistance);

                if (weakness > 1f)
                    continue;

                direction.y = 0; // i do not sure we need to use Y coord in navmesh directions, so ignoring it

                avoidanceVector += math.lerp(direction * strength, float3.zero, weakness);
            }

            AvoidanceVectors[q] = avoidanceVector;
        }
    }
}
