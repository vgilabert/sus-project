using System.Collections;
using Dreamteck.Splines;
using Train.UpgradesStats;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Train
{
    [RequireComponent(typeof(SplinePositioner))]
    public abstract class Wagon : TrainPart
    {
        [Header("Wagon Stats"), Space(5)]

        [SerializeField] protected float rotationSpeed;
        
        protected TurretStat[] Stats { get; set; }
        protected bool CanShoot { get; set; } = true;
        protected int TurretLevel { get; private set; } = 1;
        protected Enemy Target { get; private set; }
        protected float ActualDamage { get; private set; }
        protected float TimeBetweenShots { get; private set; }
        protected float RangeMax { get; private set; }
        protected float RangeMin { get; private set; }

        private NativeArray<int> TargetIndex { get; set; }
        private bool IsFacingTarget { get; set; }
        private GameObject _turretObject;

        protected override void Start()
        {
            base.Start();
            _turretObject = transform.GetChild(0).gameObject;
            TargetIndex = new NativeArray<int>(1, Allocator.Persistent);
        }

        protected virtual void Update()
        {
            
            if (!Target && CrowdController.Instance.GetEnemyList().Count > 0)
            {
                FindTarget();
            }
            UpdateTurretRotation();
            if (CanShoot && Target && IsFacingTarget)
                StartCoroutine(Shoot());
        }
        
        void OnDestroy()
        {
            TargetIndex.Dispose();
        }

        protected virtual void ApplyStats(TurretStat turretStat)
        {
            ActualDamage = turretStat.damage;
            TimeBetweenShots = turretStat.timeBetweenShots;
            RangeMin = turretStat.minDistance;
            RangeMax = turretStat.maxDistance;
        }

        protected abstract IEnumerator Shoot();

        protected void FindTarget()
        {
            FindClosestTarget findClosestJob = new FindClosestTarget
            {
                TargetPositions = CrowdController.Instance.GetEnemyPositions(),
                SeekerPosition = transform.position,
                NearestTargetIndex = TargetIndex,
                maxDistance = RangeMax,
                minDistance = RangeMin

            };
            JobHandle jobHandle = findClosestJob.Schedule();
            jobHandle.Complete();

            TargetIndex = findClosestJob.NearestTargetIndex;

            if (CrowdController.Instance.GetEnemyList().Count >= (TargetIndex[0] + 1) && TargetIndex[0]>=0)
            {
                Target = CrowdController.Instance.GetEnemyList()[TargetIndex[0]];
            }
        }

        private void UpdateTurretRotation()
        {
            if (!Target) return;
            var targetDirection = Target.transform.position - transform.position;
            var newRotation = Quaternion.LookRotation(targetDirection);
            _turretObject.transform.rotation = Quaternion.Lerp(_turretObject.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
            
            IsFacingTarget = Vector3.Angle(_turretObject.transform.forward, targetDirection) < 5;
        }
        
        protected void UpgradeTurret()
        {
            if (TurretLevel < Stats.Length)
            {
                TurretLevel++;
                ApplyStats(Stats[TurretLevel - 1]);
            }
        }
        
        public void ApplyBoost(float damageBoost, float fireRateBoost)
        {
            var dmg = Stats[TurretLevel - 1].damage;
            var timeBetweenShots = Stats[TurretLevel - 1].timeBetweenShots;
            
            ActualDamage = dmg + dmg * (damageBoost / 100);
            TimeBetweenShots -= timeBetweenShots - timeBetweenShots * (fireRateBoost / 100);
        }

        public void RemoveBoost()
        {
            ActualDamage = Stats[TurretLevel - 1].damage;
            TimeBetweenShots = Stats[TurretLevel - 1].timeBetweenShots;
        }
    }
}
