using UnityEngine;

namespace Weapons
{
    public class Gun : MonoBehaviour
    {
        public Transform muzzle;
        public IProjectile projectile;

        public float msBetweenShots = 100;
        public float muzzleVelocity = 35;
        public float damage = 10;
        public float rocketHeight;

        public float nextShotTime;
        public void Shoot()
        {
            if (Time.time > nextShotTime)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;
                IProjectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
                newProjectile.Initialize(this);
                Destroy(newProjectile.gameObject, 3f);
            }
        }

        public void Shoot(Vector3 target)
        {
            if (Time.time > nextShotTime)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;
                IProjectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
                newProjectile.Initialize(this);
                newProjectile.GetComponent<Rocket>().Target = target;
            }
        }
    }
}