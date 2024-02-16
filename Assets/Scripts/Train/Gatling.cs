using System.Collections;
using Train.UpgradesStats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Train
{
    public class Gatling : Wagon
    {
        [SerializeField] private float range;

        protected override IEnumerator Shoot()
        {
            CanShoot = false;
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
