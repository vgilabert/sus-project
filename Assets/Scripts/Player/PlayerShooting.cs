using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VFX_Controllers;
using Weapons;

namespace Player
{
    public class PlayerShooting : MonoBehaviour
    {
        [SerializeField] private GameObject bulletEffect;
        
        [SerializeField]
        private float msBetweenShots = 100;
        
        [SerializeField]
        private float damage = 10;
        public float Damage
        {
            get => damage;
            private set => damage = value;
        }
        
        private float shootTimer;
        
        private bool IsShooting {get; set; }

        public void FixedUpdate()
        {
            if (IsShooting)
            {
                TriggerShoot();
            }
        }
        
        public void TriggerShoot()
        {
            if (Time.time > shootTimer)
            {
                shootTimer = Time.time + msBetweenShots / 1000;
                Vector3 shootingDirection = transform.forward;
                IDamageable[] entities = FindObjectsOfType<IDamageable>();

                List<IDamageable> entitiesInSight = new();
                foreach (IDamageable entity in entities)
                {
                    Transform enemyTransform = entity.transform;

                    Vector3 enemyDirection = (enemyTransform.position - transform.position).normalized;

                    if (Vector3.Dot(shootingDirection, enemyDirection) > 0.995f) // ajustez ce seuil selon vos besoins
                    {
                        entitiesInSight.Add(entity);
                    }
                }

                var trail = Instantiate(bulletEffect, transform.position, transform.rotation);
                var trailScript = trail.GetComponent<BulletEffect>();
                
                if (entitiesInSight.Count == 0)
                {
                    trailScript.SetTargetPosition(transform.position + shootingDirection * 50);
                    return;
                }


                IDamageable closestEnemy = null;
                float closestDistance = float.MaxValue;
                foreach (IDamageable enemy in entitiesInSight)
                {
                    if (enemy == null) continue;
                    float distance = Vector3.Distance(enemy.transform.position, transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
                
                if (!closestEnemy)
                    return;
                
                trailScript.SetTargetPosition(closestEnemy.transform.position);
                closestEnemy.TakeHit(Damage, new RaycastHit());
                
            }

        }

        public void Shoot(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsShooting = true;
            }
            else if (context.canceled)
            {
                IsShooting = false;
            }
        }
    }
}
