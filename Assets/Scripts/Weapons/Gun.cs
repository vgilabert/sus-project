using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    public class Gun : MonoBehaviour
    {
        public Transform muzzle;
        public IProjectile projectile;

        
        [SerializeField]
        private float msBetweenShots = 100;
        [SerializeField]
        
        private float damage = 10;
        public float Damage
        {
            get => damage;
            private set => damage = value;
        }
        
        [Header("Gatling Settings")]
        
        [SerializeField]
        private float muzzleVelocity = 35;
        public float MuzzleVelocity
        {
            get => muzzleVelocity;
            private set => muzzleVelocity = value;
        }
        
        [Header("Rocket Settings")]
        
        [SerializeField]
        private float rocketHeight;
        public float RocketHeight
        {
             get => rocketHeight;
             private set => rocketHeight = value;
        }
        
        [SerializeField]
        private float flyDuration;
        public float FlyDuration
        {
            get => flyDuration;
            private set => flyDuration = value;
        }

        private float shootTimer;

        public void Shoot(Transform target = null)
        {
            if (Time.time > shootTimer)
            {
                shootTimer = Time.time + msBetweenShots / 1000;
                IProjectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
                newProjectile.Initialize(this, target);
            }
        }
    }
}