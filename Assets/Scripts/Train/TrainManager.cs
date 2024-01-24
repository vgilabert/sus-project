using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public enum CartType
{
    Missile,
    Gatling,
    Locomotive
}

public class TrainManager : MonoBehaviour
{
    List<GameObject> carts;
    
    [Header("Carts Prefabs"), Space(5)]
    [SerializeField] GameObject locomotivePrefab;
    [SerializeField] GameObject cartPrefab;
    [SerializeField] GameObject missileTurretPrefab;
    [SerializeField] GameObject gatlingTurretPrefab;
    
    [Space(15)]
    
    [SerializeField] SplineComputer spline;
    [SerializeField] float speed;
    
    GameObject locomotive;
    SplineFollower locomotiveFollower;
    
    void Start()
    {
        carts = new List<GameObject>();
        InitializeLocomotive();
    }
    
    void InitializeLocomotive()
    {
        locomotive = Instantiate(locomotivePrefab);
        locomotiveFollower = locomotive.GetComponent<SplineFollower>();
        locomotive.transform.position = spline.EvaluatePosition(0);
        locomotiveFollower.spline = spline;
        locomotiveFollower.followSpeed = speed;
        locomotive.GetComponent<Cart>().cartType = CartType.Locomotive;
        carts.Add(locomotive);
    }
    
    void AddCart(bool isGatling)
    {
        GameObject cart = Instantiate(cartPrefab, transform);
        SetCartSpline(cart);
        if (isGatling)
        {
            cart.GetComponent<Cart>().cartType = CartType.Gatling;
            SetTurretController(cart, gatlingTurretPrefab);
        }
        else
        {
            cart.GetComponent<Cart>().cartType = CartType.Missile;
            SetTurretController(cart, missileTurretPrefab);
        }
        carts.Add(cart);
    }

    void SetCartSpline(GameObject cart)
    {
        SplinePositioner splinePositioner = cart.GetComponent<SplinePositioner>();
        splinePositioner.spline = spline;
        splinePositioner.followTarget = carts[^1].GetComponent<SplineTracer>();
        splinePositioner.followTargetDistance = carts.Count == 1 ? 2 : 1.5f;
    }

    void SetTurretController(GameObject cart, GameObject turretPrefab)
    {
        TurretController controller = cart.AddComponent<TurretController>();
        controller.SetControllerData(cart, turretPrefab);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddCart(false);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AddCart(true);
        }
    }
}