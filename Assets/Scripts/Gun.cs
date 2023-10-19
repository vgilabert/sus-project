using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public float damage = 10;

    public float nextShotTime;

    private void FixedUpdate()
    {
        
    }
    public void Shoot()
    {

        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
            
            newProjectile.SetSpeed(muzzleVelocity);
            newProjectile.damage = damage;  
            Destroy(newProjectile.gameObject, 3f);
        }
    }
}
