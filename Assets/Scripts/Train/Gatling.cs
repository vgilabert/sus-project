using System.Collections;
using Train.UpgradesStats;
using UnityEngine;
using UnityEngine.Serialization;
using VFX_Controllers;

namespace Train
{
    public class Gatling : Wagon
    {
        [SerializeField] private GameObject BulletTrailEffect;
        
        protected override IEnumerator Shoot()
        {
            CanShoot = false;
            var trail = Instantiate(BulletTrailEffect, transform.GetChild(0).position, transform.GetChild(0).rotation);
            Destroy(trail, 0.5f);
            var trailScript = trail.GetComponent<BulletEffect>();
            trailScript.SetTargetPosition(Target.transform.position);
            Target.TakeHit(ActualDamage);
            
            yield return new WaitForSeconds(TimeBetweenShots);
            CanShoot = true;
        }

        #region Debug
        
        [Header("Debug"), Space(5)]
        [SerializeField] protected bool showRangeGizmos;
        private void OnDrawGizmos()
        {
            if (!showRangeGizmos) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, turretStats[TurretLevel - 1].maxDistance);
        }
        
        #endregion
        
    }
}
