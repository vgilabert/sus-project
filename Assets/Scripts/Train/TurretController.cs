using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    protected Cart cart;
    GameObject turret;

    [SerializeField] Transform turretAnchor;

    public void SetControllerData(Cart c, GameObject turretPrefab)
    {
        cart = c;
        turret = Instantiate(turretPrefab, turretAnchor.position, Quaternion.identity, transform);
    }
}