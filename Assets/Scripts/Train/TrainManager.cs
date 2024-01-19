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
    [SerializeField] GameObject gatlingPrefab;
    [SerializeField] GameObject missilePrefab;
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
        carts.Add(locomotive);
    }
    
    void AddCart(bool isGatling)
    {
        GameObject cart = Instantiate(isGatling ? gatlingPrefab : missilePrefab, transform);
        SetCartSpline(cart);
        SetTurretController(cart, isGatling ? CartType.Gatling : CartType.Missile);
        carts.Add(cart);
    }

    void SetCartSpline(GameObject cart)
    {
        SplinePositioner splinePositioner = cart.GetComponent<SplinePositioner>();
        splinePositioner.spline = spline;
        splinePositioner.followTarget = carts[^1].GetComponent<SplineTracer>();
        splinePositioner.followTargetDistance = carts.Count == 1 ? 2 : 1.5f;
    }

    void SetTurretController(GameObject cart, CartType type)
    {
        TurretController controller = cart.AddComponent<TurretController>();
        controller.SetControllerData(cart.GetComponent<Cart>(), type == CartType.Gatling ? gatlingTurretPrefab : missileTurretPrefab);
    }
}