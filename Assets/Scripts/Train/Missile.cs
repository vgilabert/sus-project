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
            
            var timeToWait = trailScript.FlightDuration / 2;
            yield return new WaitForSeconds(timeToWait);
            
            ProcessExplosion();

            yield return new WaitForSeconds(TimeBetweenShots - timeToWait);
            CanShoot = true;
        }

        private void ProcessExplosion()
        {
            // Get the list of enemies
            List<Enemy> enemyList = CrowdController.Instance.GetEnemyList();

            // Create a copy of the enemy list
            List<Enemy> enemiesCopy = new List<Enemy>(enemyList);

            // Loop through the copied list to avoid modification during iteration
            foreach (Enemy enemy in enemiesCopy)
            {
                // Calculate the distance between the explosion and the enemy
                float distanceToEnemy = Vector3.Distance(Target.transform.position, enemy.transform.position);

                // Check if the enemy is within the explosion radius
                if (distanceToEnemy <= explosionRadius)
                {
                    // Apply damage to the enemy
                    enemy.TakeHit(ActualDamage);

                    // Optionally, remove the enemy from the list if needed
                    // CrowdController.Instance.RemoveAgent(enemy);
                }
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
