using System.Collections;
using Train.UpgradesStats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Train
{
    public class Gatling : Wagon
    {
        [SerializeField] private float range;

        [SerializeField] private GameObject BulletTrailEffect;
        
        protected override IEnumerator Shoot()
        {
            CanShoot = false;
            var trail = Instantiate(BulletTrailEffect, transform.GetChild(0).position, transform.GetChild(0).rotation);
            var trailScript = trail.GetComponent<LineTrace>();
            trailScript.SetTargetPosition(Target.transform.position);
            Target.TakeHit(ActualDamage, new RaycastHit());
            
            yield return new WaitForSeconds(TimeBetweenShots);
            CanShoot = true;
        }

        #region Debug
        
        [Header("Debug"), Space(5)]
        [SerializeField] protected bool showRangeGizmos;
        private void OnDrawGizmos()
        {
            if (!showRangeGizmos) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
        
        #endregion
        
    }
}
