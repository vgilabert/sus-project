using System.Collections;
using Train.UpgradesStats;
using UnityEngine;
using VFX_Controllers;

namespace Train
{
    public class Missile : Wagon
    {
        private float explosionRadius;
        
        [SerializeField] private float rangeMin;
        [SerializeField] private float rangeMax;
        
        [SerializeField] private GameObject RocketEffect;

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
            
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                var distance = Vector3.Distance(Target.transform.position, enemy.transform.position);
                if (distance <= explosionRadius)
                {
                    StartCoroutine(ShowExplosion(enemy.transform.position));
                    if (enemy)
                        enemy.GetComponent<Enemy>().TakeHit(ActualDamage, new RaycastHit());
                }
            }

            yield return new WaitForSeconds(TimeBetweenShots - timeToWait);
            CanShoot = true;
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
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, rangeMin);
            Gizmos.DrawWireSphere(transform.position, rangeMax);
            
            if (showExplosion)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(explosionPosition, explosionRadius);
            }
        }
            
        #endregion
        
    }
}
