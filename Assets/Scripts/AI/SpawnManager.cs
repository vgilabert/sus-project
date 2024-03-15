using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using Utils;
// ReSharper disable All

namespace AI
{
    public class SpawnManager : MonoSingleton<SpawnManager>
    {
        public GameObject spawnerPrefab;
        public GameObject dronePrefab;
        
        [Header("Spawn Settings")]
        public bool useFixedAmount = false;
        public int fixAmount = 500;
        
        public int baseAmount = 1;
        public int baseIncrement = 1;
        public int multiplier = 1;
        public bool useDelay = false;
        public float spawnDelay = 0.0001f;
        
        
        
        private List<Spawner> _spawners = new();
        private SplineComputer _spline;
        
        private double _lastTriggerPositionPercent = 0;

        private void FixedUpdate()
        {
            if (_spline)
            {
                _spline.CheckTriggers(_lastTriggerPositionPercent, ProgressManager.Instance.GetProgress());
            }
        }

        public void GenerateSpawners(SplineComputer spline)
        {
            _spline = spline;
            AddTriggers(spline);
            SpawnManager spawnManager = FindFirstObjectByType<SpawnManager>();
            SplinePoint[] points = spline.GetPoints();

            int enemyCount = baseAmount;
            
            for (int i = 0; i < points.Length; i++)
            {   
                //var pos = spline.EvaluatePosition(i);
                var pos = ComputeSpawnerPositionLeftOrRight(points[i]);
                
                Spawner sp = Instantiate(spawnerPrefab, pos , Quaternion.identity, spawnManager.transform).GetComponent<Spawner>();
                sp.agentPrefab = spawnManager.dronePrefab;
                sp.spawnOnAwake = false;
                sp.count = useFixedAmount ? fixAmount : enemyCount;
                
                _spawners.Add(sp);
                enemyCount += baseIncrement * multiplier;
            }
        }

        public Vector3 ComputeSpawnerPositionLeftOrRight(SplinePoint point)
        {
            Vector3 pointTan = point.tangent;
            Vector3 normal = Vector3.Cross(pointTan, Vector3.up).normalized;

            Vector3 pos;
            if (Random.Range(0,2) == 1)
            {
                pos = point.position + normal * 20;
            }
            else
            {
                pos = point.position - normal * 20;
            }

            return pos;
        }

        private void AddTriggers(SplineComputer spline)
        {
            var points = spline.GetPoints();

            for (int i = 0; i < points.Length; i++)
            {
                var splineTrigger = spline.AddTrigger(0, spline.GetPointPercent(i), SplineTrigger.Type.Forward);
                splineTrigger.workOnce = true;
                int index = i; // pass the index by copy
                splineTrigger.onCross.AddListener(user => TriggerSpawnEvent(user, index));
            }

        }
    

        private void TriggerSpawnEvent(SplineUser user, int index)
        {
            var triggerPosition = _spline.GetPointPercent(index);
            _lastTriggerPositionPercent = triggerPosition;
            var spawnerIndex = index + 1;
            if (spawnerIndex >= _spawners.Count)
            {
                return;
            }
            _spawners[spawnerIndex].TriggerSpawnDelayed(spawnDelay);
        }
    }
}
