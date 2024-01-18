using UnityEngine;

namespace Weapons
{
    public abstract class IProjectile: MonoBehaviour
    {
        public LayerMask collisionMask;
        public LayerMask damageMask;
        protected float Speed { get; private set; }
        protected float BaseDamage { get; private set; }

        [SerializeField]
        protected GameObject impactEffect;

        public abstract void Update();

        public void Initialize(Gun gun)
        {
            BaseDamage = gun.damage;
            Speed = gun.muzzleVelocity;
        }

        protected virtual void CheckCollisions(float moveDistance)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
            {
                OnHit(hit);
            }
        }

        protected abstract void OnHit(RaycastHit hit);
    }
}