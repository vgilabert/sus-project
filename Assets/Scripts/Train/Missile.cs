using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [SerializeField] private GameObject RocketEffect;

        NativeArray<int> targetIndex; 
        
        protected override void InitializeTurretStats()
        {
            base.InitializeTurretStats();
            explosionRadius = turretStats[TurretLevel - 1].explosionRadius;
        }

        protected override IEnumerator Shoot()
        {
            CanShoot = false;
            var trail = Instantiate(RocketEffect, transform.GetChild(0).position, transform.GetChild(0).rotation);
            var trailScript = trail.GetComponent<RocketEffect>();
            trailScript.SetTargetPosition(Target.transform.position);
            
            var timeToWait = trailScript.FlightDuration;
            yield return new WaitForSeconds(timeToWait);
            
            ProcessExplosion();

            yield return new WaitForSeconds(TimeBetweenShots - timeToWait);
            CanShoot = true;
        }

        private void ProcessExplosion()
        {
            NativeArray<float3> enemyPositions = new NativeArray<float3>(CrowdController.Instance.GetEnemyCount(), Allocator.Persistent);
            enemyPositions.CopyFrom(CrowdController.Instance.GetEnemyPositions());

            List<Enemy> enemyIndexesAffected = new();
            
            for (int i = 0; i < enemyPositions.Length; i++)
            {
                if (Vector3.Distance(enemyPositions[i], Target.transform.position) <= explosionRadius)
                    enemyIndexesAffected.Add(CrowdController.Instance.GetEnemyList()[i]);
            }

            enemyPositions.Dispose();

            foreach (var enemy in enemyIndexesAffected)
            {
                enemy.TakeHit(ActualDamage);
            }
        }
        
        private void OnDestroy()
        {
            targetIndex.Dispose();
        }
        
        #region Debug

        [Header("Debug"), Space(5)]
        [SerializeField] protected bool showRangeGizmos;
        
        private bool showExplosion;
        private Vector3 explosionPosition;
        
        IEnumerator ShowExplosion(Vector3 position)
        {
            showExplosion = true;
            explosionPosition = position;
            yield return new WaitForSeconds(TimeBetweenShots);
            showExplosion = false;
        }
        
        private void OnDrawGizmos()
        {
            if (!showRangeGizmos) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, turretStats[TurretLevel - 1].maxDistance);
            Gizmos.DrawWireSphere(transform.position, turretStats[TurretLevel - 1].minDistance);
            
            if (showExplosion)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(explosionPosition, explosionRadius);
            }
        }
            
        #endregion
        
    }
}
