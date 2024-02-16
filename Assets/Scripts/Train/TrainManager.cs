using System.Collections.Generic;
using Dreamteck.Splines;
using Items;
using Train;
using UnityEngine;


public enum WagonType
{
    Missile,
    Gatling,
}

public class TrainManager : IDamageable
{
    private GameObject engine;
    private List<Wagon> wagons;
    private SplineFollower engineFollower;

    
    [Header("Prefabs"), Space(5)]
    [SerializeField] GameObject enginePrefab;
    [SerializeField] GameObject missilePrefab;
    [SerializeField] GameObject gatlingPrefab;
    
    [Space(15)]
    
    [SerializeField] SplineComputer spline;
    [SerializeField] float speed;
    
    protected void OnEnable()
    {
        TrainBoostFlow.OnTrainBoostStart += BoostStartedHandler;
        TrainBoostFlow.OnTrainBoostEnd += BoostEndedHandler;
        RepairKitFlow.OnRepairKitUsed += RepairKitUsedHandler;
    }
    
    protected override void Start()
    {
        base.Start();
        wagons = new List<Wagon>();
        InitializeEngine();
    }
    
    void InitializeEngine()
    {
        engine = Instantiate(enginePrefab, transform);
        engine.transform.position = spline.EvaluatePosition(0);
        engineFollower = engine.GetComponent<SplineFollower>();
        engineFollower.spline = spline;
        engineFollower.followSpeed = speed;
    }

    void AddCart(WagonType type)
    {
        Wagon wagon;
        
        switch (type)
        {
            case WagonType.Missile:
                wagon = Instantiate(missilePrefab, transform).GetComponent<Wagon>();
                break;
            case WagonType.Gatling:
                wagon = Instantiate(gatlingPrefab, transform).GetComponent<Wagon>();
                break;
            default:
                return;
        }
        SetPositionOnSpline(wagon);
        wagons.Add(wagon);
    }

    void SetPositionOnSpline(Wagon wagon)
    {
        SplinePositioner splinePositioner = wagon.GetComponent<SplinePositioner>();
        splinePositioner.spline = spline;
        if (wagons.Count == 0)
        {
            splinePositioner.followTarget = engine.GetComponent<SplineTracer>();
        }
        else
        {
            splinePositioner.followTarget = wagons[^1].GetComponent<SplineTracer>();
        }
        splinePositioner.followTargetDistance = wagons.Count == 0 ? 2 : 1.5f;
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
    
    protected void BoostStartedHandler(float damageBoost, float fireRateBoost)
    {
        foreach (var wagon in wagons)
        {
            wagon.ApplyBoost(damageBoost, fireRateBoost);
        }
    }

    protected void BoostEndedHandler()
    {
        foreach (var wagon in wagons)
        {
            wagon.RemoveBoost();
        }
    }
    
    protected void RepairKitUsedHandler(float repairAmount)
    {
        if (health < MaxHealth)
        {
            health += repairAmount;
        }
        
    }
}