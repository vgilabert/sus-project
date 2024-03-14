using System;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace AI
{
    public class SpawnManager : MonoSingleton<SpawnManager>
    {
        public GameObject spawnerPrefab;
        public GameObject dronePrefab;
        public int baseSpawnCount;
        public int spawnCountIncrease = 1;
        public float spawnDelay = 0.0001f;
        
        public bool useLinear = false;
        
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
            var spawnManager = FindFirstObjectByType<SpawnManager>();
            var points = spline.GetPoints();

            for (int i = 0; i < points.Length; i++)
            {
                var pos = spline.EvaluatePosition(i);
                Spawner sp = Instantiate(spawnerPrefab, pos , Quaternion.identity, spawnManager.transform).GetComponent<Spawner>();
                if (useLinear)
                {
                    // increase the spawn count using fibonacci sequence
                    sp.count = (int)(i + 0.1f * i);
                }
                else
                {
                    sp.count = (int) (baseSpawnCount + (i * spawnCountIncrease));
                }
                sp.agentPrefab = spawnManager.dronePrefab;
                sp.spawnOnAwake = false;
                _spawners.Add(sp);
            }
        }

        private void AddTriggers(SplineComputer spline)
        {
            var points = spline.GetPoints();

            for (int i = 1; i < points.Length; i++)
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
