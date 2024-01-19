using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class TurretController : MonoBehaviour
{
    GameObject turret;
    Gun gun;
    Cart cart;

    Transform turretAnchor;

    public void SetControllerData(GameObject c, GameObject turretPrefab)
    {
        cart = c.GetComponent<Cart>();
        turretAnchor = transform.GetChild(0).transform;
        turret = Instantiate(turretPrefab, turretAnchor.position, Quaternion.identity, transform);
        gun = turret.GetComponent<Gun>();
    }

    void Update()
    {
        
        if (cart.cartType == CartType.Gatling)
        {
            Aim();
            gun.Shoot();
        }
        else if (cart.cartType == CartType.Missile)
        {
            Vector3 targetPostition = GetTargetPosition();
            turret.transform.LookAt(targetPostition);
            gun.Shoot(targetPostition);
        }
    }

    protected virtual void Aim()
    {
        GameObject coucou = GameObject.Find("coucou");
        Vector3 targetPostition = new Vector3(coucou.transform.position.x, turret.transform.position.y, coucou.transform.position.z);
        turret.transform.LookAt(targetPostition);
    }
    
    protected virtual Vector3 GetTargetPosition()
    {
        GameObject coucou = GameObject.Find("coucou");
        return new Vector3(coucou.transform.position.x, turret.transform.position.y, coucou.transform.position.z);
    }
}