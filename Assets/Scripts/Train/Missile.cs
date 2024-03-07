using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Train.UpgradesStats;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VFX_Controllers;

namespace Train
{
    public class Missile : Wagon
    {
        private float explosionRadius;
        private float flightDuration;

        [SerializeField] private GameObject RocketEffect;

        NativeArray<int> targetIndex; 
        
        protected override void Start()
        {
            base.Start();
            Stats = trainManager.missileStats;
            ApplyStats(Stats[TurretLevel - 1]);
            
        }
        
        protected override void ApplyStats(TurretStat turretStat)
        {
            base.ApplyStats(turretStat);
            explosionRadius = turretStat.explosionRadius;
            flightDuration = turretStat.flightDuration;
        }

        protected override IEnumerator Shoot()
        {
            CanShoot = false;
            var trail = Instantiate(RocketEffect, transform.GetChild(0).position, transform.GetChild(0).rotation);
            var trailScript = trail.GetComponent<RocketEffect>();
            trailScript.SetTargetPosition(Target.transform.position);
            trailScript.SetFlightDuration(flightDuration);
            
            var timeToWait = flightDuration;
            yield return new WaitForSeconds(timeToWait);
            
            ProcessExplosion();

            yield return new WaitForSeconds(TimeBetweenShots - timeToWait);
            CanShoot = true;
        }

        private void ProcessExplosion()
        {
            int enemyCount = CrowdController.Instance.GetEnemyCount();
            
            List<Enemy> enemyList = CrowdController.Instance.GetEnemyList().ToList();

            NativeArray<float3> enemyPositions = new NativeArray<float3>(CrowdController.Instance.GetEnemyPositions(), Allocator.Persistent);
            
            NativeArray<int> enemyIndexesAffected = new NativeArray<int>(enemyCount, Allocator.Persistent);
            
            ExplosionJob explosionJob = new ExplosionJob
            {
                explosionPosition = Target.transform.position,
                explosionRadius = explosionRadius,
                enemyPositions = enemyPositions,
                enemyIndexes = enemyIndexesAffected
            };
            
            JobHandle explosionJobHandle = explosionJob.Schedule();
            explosionJobHandle.Complete();
            
            enemyPositions.Dispose();

            for (int i = 0; i < enemyIndexesAffected.Length; i++)
            {
                if (enemyIndexesAffected[i] == -1)
                    continue;
                if (enemyList[i])
                {
                    enemyList[i].TakeHit(ActualDamage);
                }
            }
            
            enemyIndexesAffected.Dispose();
        }
        
        private void OnDestroy()
        {
            targetIndex.Dispose();
        }
        
        #region Debug

        [Header("Debug"), Space(5)]
        [SerializeField] protected bool showRangeGizmos;
        
        private bool _showExplosion;
        private Vector3 _explosionPosition;
        
        IEnumerator ShowExplosion(Vector3 position)
        {
            _showExplosion = true;
            _explosionPosition = position;
            yield return new WaitForSeconds(TimeBetweenShots);
            _showExplosion = false;
        }
        
        private void OnDrawGizmos()
        {
            if (!showRangeGizmos) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, RangeMin);
            Gizmos.DrawWireSphere(transform.position, RangeMax);
            
            if (_showExplosion)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_explosionPosition, explosionRadius);
            }
        }
            
        #endregion
        
    }
}
