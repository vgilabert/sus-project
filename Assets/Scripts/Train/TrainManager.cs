using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;


public enum WagonType
{
    Missile,
    Gatling,
}

public class TrainManager : IDamageable
{
    private GameObject engine;
    private List<GameObject> wagons;
    private SplineFollower engineFollower;

    
    [Header("Prefabs"), Space(5)]
    [SerializeField] GameObject enginePrefab;
    [SerializeField] GameObject missilePrefab;
    [SerializeField] GameObject gatlingPrefab;
    
    [Space(15)]
    
    [SerializeField] SplineComputer spline;
    [SerializeField] float speed;
    
    protected override void Start()
    {
        base.Start();
        wagons = new List<GameObject>();
        InitializeEngine();
    }
    
    void InitializeEngine()
    {
        engine = Instantiate(enginePrefab, transform);
        engine.transform.position = spline.EvaluatePosition(0);
        engineFollower = engine.GetComponent<SplineFollower>();
        engineFollower.spline = spline;
        engineFollower.followSpeed = speed;
        wagons.Add(engine);
    }

    void AddCart(WagonType type)
    {
        GameObject wagon;
        
        switch (type)
        {
            case WagonType.Missile:
                wagon = Instantiate(missilePrefab, transform);
                break;
            case WagonType.Gatling:
                wagon = Instantiate(gatlingPrefab, transform);
                break;
            default:
                return;
        }
        SetPositionOnSpline(wagon);
        wagons.Add(wagon);
    }

    void SetPositionOnSpline(GameObject cart)
    {
        SplinePositioner splinePositioner = cart.GetComponent<SplinePositioner>();
        splinePositioner.spline = spline;
        splinePositioner.followTarget = wagons[^1].GetComponent<SplineTracer>();
        splinePositioner.followTargetDistance = wagons.Count == 1 ? 2 : 1.5f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddCart(WagonType.Gatling);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            AddCart(WagonType.Missile);
        }
    }
}