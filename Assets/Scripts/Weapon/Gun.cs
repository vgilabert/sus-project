using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MuzzleFlash))]
public class Gun : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform muzzle;
    public MuzzleFlash flash;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public float damage = 10;

    public float nextShotTime;

    private void Start()
    {
        flash = GetComponent<MuzzleFlash>();
    }
    private void FixedUpdate()
    {
        
    }
    public void Shoot()
    {

        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);

            flash.Activate();
            newProjectile.SetSpeed(muzzleVelocity);
            newProjectile.damage = damage;  
            Destroy(newProjectile.gameObject, 3f);
        }
    }
}
