using System;
using System.Collections;
using Dreamteck.Splines;
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

        protected override void Start()
        {
            base.Start();
            _turret = transform.GetChild(0).gameObject;
        }
        
        protected virtual void Update()
        {
            FindTarget();
            UpdateTurretRotation();
            if (CanShoot && Target && IsFacingTarget)
                StartCoroutine(Shoot());
        }

        protected abstract IEnumerator Shoot();

        protected virtual void FindTarget()
        {
            if (!Target) return;
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0) return;
            var closest = enemies[0];
            var closestDistance = Vector3.Distance(transform.position, closest.transform.position);
            foreach (var enemy in enemies)
            {
                var distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closest = enemy;
                    closestDistance = distance;
                }
            }
            Target = closest.GetComponent<Enemy>();
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
