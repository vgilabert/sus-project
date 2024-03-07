using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using VFX_Controllers;
using Weapons;

namespace Character
{
    public class PlayerShooting : MonoBehaviour
    {
        [SerializeField] 
        private GameObject bulletEffect;
        [SerializeField]
        private float msBetweenShots = 100;
        [SerializeField]
        private float damage = 10;
        [SerializeField]
        private float Range = 5;
        
        
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

                var target = FindTarget(shootingDirection);

                var trail = Instantiate(bulletEffect, transform.position, Quaternion.identity);
                var trailScript = trail.GetComponent<BulletEffect>();
                
                if (target)
                {
                    trailScript.SetTargetPosition(target.transform.position);
                    target.TakeHit(Damage);
                }
                else
                {
                    trailScript.SetTargetPosition(shootingDirection * Range + transform.position);
                }
                
                Destroy(trail, 1.5f);
            }
        }

        private Enemy FindTarget(Vector3 shootingDirection)
        {
            var enemyCount = CrowdController.Instance.GetEnemyCount();
            if (enemyCount == 0)
                return null; 

            List<Enemy> enemies = CrowdController.Instance.GetEnemyList();

            NativeArray<int> nearestTargetIndex = new NativeArray<int>(1, Allocator.Persistent);
            
            NativeArray<float3> enemyPositionsInSight = new NativeArray<float3>(enemyCount, Allocator.Persistent);
            bool found = false;

            for (int i = 0; i < enemyCount; i++)
            {
                var e = enemies[i];
                if (e)
                {
                    var direction = e.transform.position - transform.position;
                    if (Vector3.Dot(Vector3.Normalize(direction), Vector3.Normalize(shootingDirection)) > 0.99f)
                    {
                        enemyPositionsInSight[i] = e.transform.position;
                        found = true;
                    }
                }
            }

            if (!found)
            {
                nearestTargetIndex.Dispose();
                return null;
            }

            FindClosestTarget findClosestJob = new FindClosestTarget
            {
                TargetPositions = enemyPositionsInSight,
                SeekerPosition = transform.position,
                NearestTargetIndex = nearestTargetIndex,
                maxDistance = Range,
                minDistance = 0
            };
            JobHandle jobHandle = findClosestJob.Schedule();
            jobHandle.Complete();
            
            int index = nearestTargetIndex[0];
            if (index < 0)
            {
                nearestTargetIndex.Dispose();
                return null;
            }
            Enemy enemy = enemies[index];
            nearestTargetIndex.Dispose();
            return enemy;
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
