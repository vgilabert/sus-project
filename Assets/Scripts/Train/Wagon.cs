using System.Collections;
using Dreamteck.Splines;
using Train.UpgradesStats;
using Unity.Jobs;
using UnityEngine;

namespace Train
{
    [RequireComponent(typeof(SplinePositioner))]
    public abstract class Wagon : TrainPart
    {
        protected GameObject _turretObject;

        protected int TurretLevel { get; set; } = 1;
        protected float ActualDamage { get; set; }
        protected float TimeBetweenShots { get; set; }
        protected Enemy Target { get; set; }
        protected bool IsFacingTarget { get; set; }
        protected bool CanShoot { get; set; } = true;

        [Header("Wagon Stats"), Space(5)]

        [SerializeField] protected float rotationSpeed;
        
        [SerializeField] protected TurretStat[] turretStats;
        
        int TargetIndex { get; set; }

        protected override void Start()
        {
            base.Start();
            _turretObject = transform.GetChild(0).gameObject;
            InitializeTurretStats();
        }
        
        protected virtual void Update()
        {
            if (!Target)
                FindTarget();
            UpdateTurretRotation();
            if (CanShoot && Target && IsFacingTarget)
                StartCoroutine(Shoot());
        }

        protected virtual void InitializeTurretStats()
        {
            ActualDamage = turretStats[TurretLevel - 1].damage;
            TimeBetweenShots = turretStats[TurretLevel - 1].timeBetweenShots;
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
            _turretObject.transform.rotation = Quaternion.Lerp(_turretObject.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
            
            IsFacingTarget = Vector3.Angle(_turretObject.transform.forward, targetDirection) < 5;
        }
        
        protected void UpgradeTurret()
        {
            if (TurretLevel < turretStats.Length)
            {
                TurretLevel++;
            }
        }
        
        public void ApplyBoost(float damageBoost, float fireRateBoost)
        {
            var dmg = turretStats[TurretLevel - 1].damage;
            var timeBetweenShots = turretStats[TurretLevel - 1].timeBetweenShots;
            
            ActualDamage = dmg + dmg * (damageBoost / 100);
            TimeBetweenShots -= timeBetweenShots - timeBetweenShots * (fireRateBoost / 100);
        }

        public void RemoveBoost()
        {
            ActualDamage = turretStats[TurretLevel - 1].damage;
            TimeBetweenShots = turretStats[TurretLevel - 1].timeBetweenShots;
        }
    }
}
