using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

[RequireComponent(typeof(SplinePositioner))]
public class Cart : IDamageable
{
    public CartType cartType{ get; set; }
    
    protected int ameliorationLVL;
    
    protected float fireRate;
    protected float range;
    protected float damage;

    protected bool passiveUnlocked;

    public override void TakeHit(float dmg, RaycastHit hit, Vector3 hitDirection = default)
    {
        transform.GetComponentInParent<TrainManager>().TakeHit(dmg, hit, hitDirection);
    }
}
