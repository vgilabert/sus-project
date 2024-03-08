using System.Collections.Generic;
using Dreamteck.Splines;
using Items;
using Character;
using Train;
using Train.UpgradesStats;
using UnityEngine;
using UnityEngine.Serialization;


public enum WagonType
{
    Rocket,
    Gatling,
    Engine
}

public class TrainManager : IDamageable
{
    private GameObject engine;
    private List<Wagon> wagons;
    public List<Wagon> Wagons => wagons;
    private SplineFollower engineFollower;
    private Inventory _playerInventory;
    
    [Header("Cart Stats"), Space(5)]
    [SerializeField] public TurretStat[] gatlingStats;
    [SerializeField] public TurretStat[] missileStats;
    [SerializeField] public EngineStat[] engineStats;
    
    [Header("Prefabs"), Space(5)]
    [SerializeField] GameObject enginePrefab;
    [SerializeField] GameObject missilePrefab;
    [SerializeField] GameObject gatlingPrefab;
    
    [Space(15)]
    
    [SerializeField] SplineComputer spline;
    [SerializeField] float speed;

    [SerializeField] private int power;

    protected void OnEnable()
    {
        //TODO: A mettre autre part?
        TrainBoostFlow.OnTrainBoostStart += BoostStartedHandler;
        TrainBoostFlow.OnTrainBoostEnd += BoostEndedHandler;
        RepairKitFlow.OnRepairKitUsed += RepairKitUsedHandler;
    }
    
    protected override void Start()
    {
        base.Start();
        wagons = new List<Wagon>();
        _playerInventory = FindFirstObjectByType<Player>().GetComponentInChildren<Inventory>();
        power = engineStats[0].power;
        InitializeEngine();
    }

    void InitializeEngine()
    {
        engine = Instantiate(enginePrefab, transform);
        engine.transform.position = spline.EvaluatePosition(0);
        engineFollower = engine.GetComponent<SplineFollower>();
        engineFollower.spline = spline;
        engineFollower.followSpeed = engineStats[0].speed;
        MaxHealth = engineStats[0].maxHealth;
    }

    public void BuyGatling()
    {
        BuyWagon(WagonType.Gatling);
    }
    
    public void BuyRocket()
    {
        BuyWagon(WagonType.Rocket);
    }
    
    private void BuyWagon(WagonType type)
    {
        int cost = int.MaxValue;
        GameObject prefab = null;
        switch (type)
        {
            case WagonType.Gatling:
                prefab = gatlingPrefab;
                cost = gatlingStats[0].scrapCost;
                break;
            case WagonType.Rocket:
                prefab = missilePrefab;
                cost = missileStats[0].scrapCost;
                break;
            default:
                return;
        }
        if (_playerInventory.TrySpendScarp(cost))
        {
            BuildWagon(prefab);
        }
        else
        {
            Debug.Log("Not enough scrap (" + _playerInventory.Scrap + " < )" + cost); 
        }
    }
    
    private void BuildWagon(GameObject prefab)
    {
        Wagon wagon = Instantiate(prefab, transform).GetComponent<Wagon>();
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
        var enginePos = engineFollower.result.percent;
        ProgressManager.Instance.UpdateProgress(enginePos);
        if (Input.GetKeyDown(KeyCode.E))
        {
            BuyWagon(WagonType.Gatling);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            BuyWagon(WagonType.Rocket);
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

    public int GetAvailablePower()
    {
        int availablePower = power;
        foreach (var wagon in wagons)
        {
            availablePower -= wagon.GetPowerCost();
        }
        return availablePower;
    }
    public void UpgradeWagon(Wagon wagon)
    {
        wagon.UpgradeTurret();
    }
}