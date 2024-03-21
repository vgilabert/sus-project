using System.Collections;
using Train.Upgrades;
using UnityEngine;
using VFX_Controllers;

namespace Train
{
    public class Gatling : Turret
    {
        private GatlingUpgrade GatlingUpgrade => (GatlingUpgrade) Upgrades[Level];

        protected override IEnumerator Shoot()
        {
            CanShoot = false;
            var trail = Instantiate(CurrentProjectileEffect, transform.GetChild(0).position, transform.GetChild(0).rotation);
            Destroy(trail, 0.5f);
            var trailScript = trail.GetComponent<BulletEffect>();
            if (trailScript)
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
            Gizmos.DrawWireSphere(transform.position, GatlingUpgrade.MaxDistance);
        }
        
        #endregion
        
    }
}
