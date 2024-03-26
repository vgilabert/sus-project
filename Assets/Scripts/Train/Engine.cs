using System;
using System.Collections.Generic;
using Dreamteck.Splines;
using Train.Upgrades;
using UnityEngine;

namespace Train
{
    public class Engine : TrainPart
    {
        private SplineFollower _follower;
        public SplineFollower Follower => _follower;
        
        private EngineUpgrade[] EngineUpgrades => (EngineUpgrade[]) Upgrades;


        protected void Awake()
        {
            _follower = GetComponent<SplineFollower>();
        }

        protected override void Start()
        {
            base.Start();
            _follower.onNode += OnNode;
        }

        private void OnNode(List<SplineTracer.NodeConnection> nodeConnection)
        {
            Debug.Log("on node");
        }

        public void Initialize(SplineComputer spline, EngineUpgrade[] upgrades)
        {
            Upgrades = upgrades;
            _follower.spline = spline;
            ApplyUpgrade();
        }
        
        protected override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            _follower.followSpeed = EngineUpgrades[Level].speed;
        }
    }
}
