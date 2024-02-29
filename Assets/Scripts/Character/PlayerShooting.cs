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
                    trailScript.SetTargetPosition(target.transform.position);
                }
            }
        }

        private Enemy FindTarget(Vector3 shootingDirection)
        {
            NativeArray<float3> enemies = CrowdController.Instance.GetEnemyPositions();
            var enemyCount = CrowdController.Instance.GetEnemyCount();
            if (enemyCount == 0)
                return null;
            NativeArray<float3> entitiesInSight = new NativeArray<float3>(enemyCount, Allocator.Persistent);
            
            for(int i = 0; i < enemyCount-1; i++)
            {
                float3 position = new float3(transform.position.x, transform.position.y, transform.position.z);
                float3 enemyDirection = math.abs(enemies[i] - position);
                Debug.Log(Vector3.Dot(shootingDirection, enemyDirection));
                
                if (Vector3.Dot(shootingDirection, enemyDirection) > 0.995f)
                {
                    entitiesInSight[i] = enemies[i];
                }
            }
            if (entitiesInSight.Length == 0)
            {
                return null;
            }

            NativeArray<int> nearestTargetIndex = new NativeArray<int>(1, Allocator.Persistent);
            FindClosestTarget findClosestJob = new FindClosestTarget
            {
                TargetPositions = entitiesInSight,
                SeekerPosition = transform.position,
                NearestTargetIndex = nearestTargetIndex,
                maxDistance = 50,
                minDistance = 0
            };
            JobHandle jobHandle = findClosestJob.Schedule();
            jobHandle.Complete();
            
            var enemy = CrowdController.Instance.GetEnemyList()[nearestTargetIndex[0]];
            nearestTargetIndex.Dispose();
            entitiesInSight.Dispose();
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
