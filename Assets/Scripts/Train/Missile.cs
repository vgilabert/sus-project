using System.Collections;
using UnityEngine;

namespace Train
{
    public class Missile : Wagon
    {
        [SerializeField] private float rangeMin;
        [SerializeField] private float rangeMax;

        [Header("Debug"), Space(5)]
        [SerializeField] protected bool showRangeGizmos;

        protected override IEnumerator Shoot()
        {
            while (Target)
            {
                if (IsFacingTarget)
                {
                    var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (var enemy in enemies)
                    {
                        var distance = Vector3.Distance(transform.position, enemy.transform.position);
                        if (distance >= rangeMin && distance <= rangeMax)
                        {
                            enemy.GetComponent<Enemy>().TakeHit(damage, new RaycastHit());
                        }
                    }
                }
                yield return new WaitForSeconds(timeBetweenShots);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (!showRangeGizmos) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, rangeMin);
            Gizmos.DrawWireSphere(transform.position, rangeMax);
        }
    }
}
