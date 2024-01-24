using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

[RequireComponent(typeof(SplinePositioner))]
public class Cart : MonoBehaviour
{
    public CartType cartType{ get; set; }
    
    protected int ameliorationLVL;
    
    protected float fireRate;
    protected float range;
    protected float damage;

    protected bool passiveUnlocked;
}
