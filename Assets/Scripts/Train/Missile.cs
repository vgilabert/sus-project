using System.Collections;
using UnityEngine;

namespace Train
{
    public class Missile : Wagon
    {
        [SerializeField] private float explosionRadius;
        
        [SerializeField] private float rangeMin;
        [SerializeField] private float rangeMax;

        protected override IEnumerator Shoot()
        {
            CanShoot = false;
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                var distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= explosionRadius)
                {
                    StartCoroutine(ShowExplosion(enemy.transform.position));
                    enemy.GetComponent<Enemy>().TakeHit(damage, new RaycastHit());
                }
            }

            yield return new WaitForSeconds(timeBetweenShots);
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
            yield return new WaitForSeconds(timeBetweenShots);
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
