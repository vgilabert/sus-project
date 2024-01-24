using UnityEngine;

namespace Weapons
{
    public abstract class IProjectile: MonoBehaviour
    {
        public LayerMask collisionMask;
        public LayerMask damageMask;
        protected float Speed { get; set; }
        protected float BaseDamage { get; set; }
        
        protected float Height { get; set; }

        [SerializeField]
        protected GameObject impactEffect;

        public abstract void Update();

        public abstract void Initialize(Gun gun, Transform target = null);

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