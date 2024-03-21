using System.Collections;
using Dreamteck.Splines;
using Train.Upgrades;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Assertions;

namespace Train
{
    [RequireComponent(typeof(SplinePositioner))]
    public abstract class Turret : TrainPart
    {
        [SerializeField] protected float rotationSpeed;
        
        [Header("Prefabs"), Space(5)]
        [SerializeField] private GameObject projectileEffect;
        [SerializeField] private GameObject projectileEffectBoosted;
        
        protected GameObject CurrentProjectileEffect;
        
        private TurretUpgrade CurrentTurretUpgrade => Upgrades[Level] as TurretUpgrade;

        protected bool CanShoot { get; set; } = true;
        protected Enemy Target { get; private set; }
        protected float ActualDamage { get; private set; }
        protected float TimeBetweenShots { get; private set; }

        private NativeArray<int> TargetIndex { get; set; }
        private bool IsFacingTarget { get; set; }
        private GameObject _turretObject;

        protected override void Start()
        {
            base.Start();
            CurrentProjectileEffect = projectileEffect;
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

        protected abstract IEnumerator Shoot();

        protected void FindTarget()
        {
            FindClosestTarget findClosestJob = new FindClosestTarget
            {
                TargetPositions = CrowdController.Instance.GetEnemyPositions(),
                SeekerPosition = transform.position,
                NearestTargetIndex = TargetIndex,
                maxDistance = CurrentTurretUpgrade.MaxDistance,
                minDistance = CurrentTurretUpgrade.MinDistance

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

        public void Initialize(TurretUpgrade[] turretUpgrades)
        {
            Upgrades = turretUpgrades;
            Level = 0;
            ApplyUpgrade();
        }
        
        protected override void ApplyUpgrade()
        {
            base.ApplyUpgrade();
            ActualDamage = CurrentTurretUpgrade.Damage;
            TimeBetweenShots = CurrentTurretUpgrade.TimeBetweenShots;
        }

        public void ApplyBoost(float damageBoost, float fireRateBoost)
        {
            CurrentProjectileEffect = projectileEffectBoosted;
            var dmg = CurrentTurretUpgrade.Damage;
            var timeBetweenShots = CurrentTurretUpgrade.TimeBetweenShots;
            
            ActualDamage = dmg + dmg * (damageBoost / 100);
            TimeBetweenShots -= timeBetweenShots - timeBetweenShots * (fireRateBoost / 100);
        }

        public void RemoveBoost()
        {
            CurrentProjectileEffect = projectileEffect;
            ActualDamage = CurrentTurretUpgrade.Damage;
            TimeBetweenShots = CurrentTurretUpgrade.TimeBetweenShots;
        }

        public int GetPowerCost()
        {
            return CurrentTurretUpgrade.PowerCost;
        }
    }
}
