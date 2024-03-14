using System;
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
    private List<Wagon> wagons;
    public List<Wagon> Wagons => wagons;
    private SplineFollower engineFollower;
    private Inventory _playerInventory;

    [Header("Train settings"), Space(5)] 
    [SerializeField]
    private float engineSpacing = 5.0f;
    [SerializeField] 
    private float wagonSpacing = 1.5f;
    
    [Header("Cart Stats"), Space(5)]
    [SerializeField] public TurretStat[] gatlingStats;
    [SerializeField] public TurretStat[] missileStats;
    [SerializeField] public EngineStat[] engineStats;
    
    [Header("Prefabs"), Space(5)]
    [SerializeField] GameObject missilePrefab;
    [SerializeField] GameObject gatlingPrefab;
    
    [Space(15)]
    
    [SerializeField] private GameObject engine;
    [SerializeField] SplineComputer spline;

    private int power;
    private float speed;
    
    //Return current wagon stat and next wagon stats
    public Action<Wagon,TurretStat,TurretStat> OnWagonBuilded;
    public Action<Wagon,TurretStat,TurretStat> OnWagonUpgraded;

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
        _playerInventory = FindFirstObjectByType<Player>().GetComponentInChildren<Inventory>();
        power = engineStats[0].power;
        InitializeEngine();
    }

    void InitializeEngine()
    {
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

    public TurretStat[] GetStatsFromType(WagonType type)
    {
        switch (type)
        {
            case WagonType.Gatling:
                return gatlingStats;
            case WagonType.Rocket:
                return missileStats;
            default:
                return null;
        }
            
    }

    public GameObject GetPrefabFromType(WagonType type)
    {
        switch (type)
        {
            case WagonType.Rocket:
                return missilePrefab;
            case WagonType.Gatling:
                return gatlingPrefab;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    private void BuyWagon(WagonType type)
    {
        TurretStat[] turretState = GetStatsFromType(type);
        if (_playerInventory.TrySpendScarp(turretState[0].scrapCost))
        {
            BuildWagon(GetPrefabFromType(type),type);
            OnWagonBuilded?.Invoke(wagons[^1],turretState[0],turretState[1]);
        }
        else
        {
            Debug.Log("Not enough scrap (" + _playerInventory.Scrap + " < )" + turretState[0].scrapCost); 
        }
    }

    
    private void BuildWagon(GameObject prefab, WagonType wagonType)
    {
        
        Wagon wagon = Instantiate(prefab, transform).GetComponent<Wagon>();
        wagon.WagonType = wagonType;
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
        splinePositioner.followTargetDistance = wagons.Count == 0 ? engineSpacing : wagonSpacing;
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

    public bool IsMaxLevel(Wagon wagon) => wagon.GetNextUpgradeStats()?.scrapCost == null;
    public void UpgradeWagon(Wagon wagon)
    {
        if (IsMaxLevel(wagon))
        {
            Debug.LogWarning("max level");
            return;
        }
        
        var cost = wagon.GetNextUpgradeStats()?.scrapCost;

        if (_playerInventory.TrySpendScarp(cost.Value))
        {
            wagon.UpgradeTurret();
            TurretStat[] turretState = GetStatsFromType(wagon.WagonType);
            int turretIndex = wagon.TurretLevel;
            TurretStat nextState = IsMaxLevel(wagon) ? null : turretState[turretIndex];
            OnWagonUpgraded?.Invoke(wagon, turretState[turretIndex-1],nextState);
        }
        
        else
        {
            Debug.Log("not enough scrap");
        }
    }
}