using System;
using System.Collections;
using Dreamteck.Splines;
using Unity.Jobs;
using UnityEngine;

namespace Train
{
    [RequireComponent(typeof(SplinePositioner))]
    public abstract class Wagon : TrainPart
    {
        protected GameObject _turret;

        protected Enemy Target { get; set; }
        protected bool IsFacingTarget { get; set; }
        protected bool CanShoot { get; set; } = true;

        [Header("Wagon Stats"), Space(5)]
        [SerializeField] protected float damage;
        [SerializeField] protected float timeBetweenShots;
        [SerializeField] protected float rotationSpeed;
        
        int TargetIndex { get; set; }

        protected override void Start()
        {
            base.Start();
            _turret = transform.GetChild(0).gameObject;
        }
        
        protected virtual void Update()
        {
            if (!Target)
                FindTarget();
            UpdateTurretRotation();
            if (CanShoot && Target && IsFacingTarget)
                StartCoroutine(Shoot());
        }

        protected abstract IEnumerator Shoot();

        protected virtual void FindTarget()
        {
            FindClosestTarget findClosestJob = new FindClosestTarget
            {
                TargetPositions = CrowdController.Instance.GetEnemyPositions(),
                SeekerPosition = transform.position,
                NearestTargetIndex = TargetIndex
            };
            
            JobHandle jobHandle = findClosestJob.Schedule();
            jobHandle.Complete();

            if (CrowdController.Instance.GetEnemyList().Count >= TargetIndex + 1)
            {
                Target = CrowdController.Instance.GetEnemyList()[TargetIndex];
            }
            
        }

        protected virtual void UpdateTurretRotation()
        {
            if (Target == null) return;
            var targetDirection = Target.transform.position - transform.position;
            var newRotation = Quaternion.LookRotation(targetDirection);
            _turret.transform.rotation = Quaternion.Lerp(_turret.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
            
            IsFacingTarget = Vector3.Angle(_turret.transform.forward, targetDirection) < 5;
        }
    }
}
