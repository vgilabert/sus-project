using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

[RequireComponent(typeof(SplinePositioner), typeof(TurretController))]
public class Cart : MonoBehaviour
{
    protected CartType cartType;
    
    protected int ameliorationLVL;
    
    protected float fireRate;
    protected float range;
    protected float damage;

    protected bool passiveUnlocked;

    [SerializeField] protected TurretController turretController;
}
