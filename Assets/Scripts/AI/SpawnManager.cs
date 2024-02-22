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
        [Range(0, 10)]
        public int baseSpawnCount;
        public float spawnCountIncrease = 1;
        
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
                sp.count = (int) (baseSpawnCount + (i * spawnCountIncrease));
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
            _spawners[spawnerIndex].TriggerSpawnDelayed(0.01f);
        }
    }
}
