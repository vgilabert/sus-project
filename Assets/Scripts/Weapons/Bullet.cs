using System;
using UnityEngine;

namespace Weapons
{
    public class Bullet : IProjectile
    {

        private void Start()
        {
            Destroy(gameObject, 3f);
        }

        public override void Update()
        {
            float moveDistance = Speed * Time.deltaTime;
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * moveDistance);
        }

        public override void Initialize(Gun gun, Transform target = null)
        {
            BaseDamage = gun.Damage;
            Speed = gun.MuzzleVelocity;
        }

        protected override void OnHit(RaycastHit hit)
        {
            IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                bool isHit = (damageMask & (1 << hit.collider.transform.gameObject.layer)) != 0;
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
