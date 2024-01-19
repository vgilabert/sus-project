using UnityEngine;

namespace Weapons
{
    public class Bullet : IProjectile
    {
        public override void Update()
        {
            float moveDistance = Speed * Time.deltaTime;
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * moveDistance);
        }

        protected override void OnHit(RaycastHit hit)
        {
            IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                bool isHit = damageMask == (damageMask | (1 << hit.collider.gameObject.layer));
                damageableObject.TakeHit(isHit?BaseDamage:0, hit, transform.forward);
            }
            else
            {
                Destroy(Instantiate(impactEffect.gameObject, hit.point, Quaternion.FromToRotation(Vector3.forward, transform.forward)), 3f);
            }
            Destroy(gameObject);
        }
    }
}
