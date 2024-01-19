using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class TurretController : MonoBehaviour
{
    GameObject turret;
    Gun gun;

    Transform turretAnchor;

    public void SetControllerData(GameObject c, GameObject turretPrefab)
    {
        turretAnchor = transform.GetChild(0).transform;
        turret = Instantiate(turretPrefab, turretAnchor.position, Quaternion.identity, transform);
        gun = turret.GetComponent<Gun>();
    }

    void Update()
    {
        Aim(); 
        gun.Shoot();
    }

    protected virtual void Aim()
    {
        GameObject coucou = GameObject.Find("coucou");
        Vector3 targetPostition = new Vector3(coucou.transform.position.x, turret.transform.position.y, coucou.transform.position.z);
        turret.transform.LookAt(targetPostition);
    }
}