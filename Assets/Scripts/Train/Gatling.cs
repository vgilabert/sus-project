using System.Collections;
using UnityEngine;

namespace Train
{
    public class Gatling : Wagon
    {
        [SerializeField] private float range;

        [Header("Debug"), Space(5)]
        [SerializeField] protected bool showRangeGizmos;
        
        protected override IEnumerator Shoot()
        {
            CanShoot = false;
            if (IsFacingTarget)
            {
                Target.TakeHit(damage, new RaycastHit());
            }
            yield return new WaitForSeconds(timeBetweenShots);
            CanShoot = true;
        }

        private void OnDrawGizmos()
        {
            if (!showRangeGizmos) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
