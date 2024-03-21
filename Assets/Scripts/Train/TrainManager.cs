using System;
using System.Collections.Generic;
using Dreamteck.Splines;
using Items;
using Character;
using Train;
using Train.Upgrades;
using UnityEngine;
using UnityEngine.Serialization;


public enum TrainType
{
    Rocket,
    Gatling,
    Engine
}

public class TrainManager : IDamageable
{
    private SplineFollower engineFollower;
    private Inventory _playerInventory;

    [Header("Train settings"), Space(5)] 
    [SerializeField]
    private float engineSpacing = 5.0f;
    [FormerlySerializedAs("wagonSpacing")] [SerializeField] 
    private float turretSpacing = 1.5f;
    
    [Header("Upgrades"), Space(5)]
    [SerializeField] public EngineUpgrades engineUpgrades;
    [SerializeField] public GatlingUpgrades gatlingUpgrades;
    [SerializeField] public RocketUpgrades rocketUpgrades;
    
    [Header("Prefabs"), Space(5)]
    [SerializeField] GameObject missilePrefab;
    [SerializeField] GameObject gatlingPrefab;
    
    [Space(15)]
    
    [SerializeField] public Engine engine;
    [SerializeField] SplineComputer spline;

    private List<Turret> _turretList;
    
    private int Power => engineUpgrades.upgrades[engine.Level].maxPower;
    
    public Action<Turret, TurretUpgrade, TurretUpgrade> OnTurretBuilt;
    public Action<Turret, TurretUpgrade, TurretUpgrade> OnTurretUpgraded;
    public Action<EngineUpgrade, EngineUpgrade> OnEngineUpgraded;

    protected void OnEnable()
    {
        TrainBoostFlow.OnTrainBoostStart += BoostStartedHandler;
        TrainBoostFlow.OnTrainBoostEnd += BoostEndedHandler;
        RepairKitFlow.OnRepairKitUsed += RepairKitUsedHandler;
    }

    protected override void Start()
    {
        base.Start();
        _turretList = new List<Turret>();
        _playerInventory = FindFirstObjectByType<Player>().GetComponentInChildren<Inventory>();
        MaxHealth = engineUpgrades.upgrades[0].maxHealth;
        Health = MaxHealth;
        engine.Initialize(spline, engineUpgrades.upgrades);
        engine.TrainType = TrainType.Engine;
        engineFollower = engine.GetComponent<SplineFollower>();
    }

    public void BuyGatling()
    {
        BuyTurret(TrainType.Gatling);
    }
    
    public void BuyRocket()
    {
        BuyTurret(TrainType.Rocket);
    }

    private Upgrade[] GetUpgradesFromType(TrainType type)
    {
        switch (type)
        {
            case TrainType.Gatling:
                return gatlingUpgrades.upgrades as GatlingUpgrade[];
            case TrainType.Rocket:
                return rocketUpgrades.upgrades as RocketUpgrade[];
            case TrainType.Engine:
                return engineUpgrades.upgrades as EngineUpgrade[];
            default:
                return null;
        }
    }

    public GameObject GetPrefabFromType(TrainType type)
    {
        switch (type)
        {
            case TrainType.Rocket:
                return missilePrefab;
            case TrainType.Gatling:
                return gatlingPrefab;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    private void BuyTurret(TrainType type)
    {
        TurretUpgrade[] turretUpgrade = GetUpgradesFromType(type) as TurretUpgrade[];
        if (turretUpgrade == null)
        {
            Debug.LogWarning("No turret upgrade found");
            return;
        }
        if (_playerInventory.TrySpendScarp(turretUpgrade[0].ScrapCost))
        {
            BuildTurret(GetPrefabFromType(type),type);
            OnTurretBuilt?.Invoke(_turretList[^1],turretUpgrade[0],turretUpgrade[1]);
        }
        else
        {
            Debug.Log("Not enough scrap (" + _playerInventory.Scrap + " < )" + turretUpgrade[0].ScrapCost); 
        }
    }

    
    private void BuildTurret(GameObject prefab, TrainType trainType)
    {
        
        Turret turret = Instantiate(prefab, transform).GetComponent<Turret>();
        turret.TrainType = trainType;
        SetPositionOnSpline(turret);
        _turretList.Add(turret);
        turret.Initialize(GetUpgradesFromType(trainType) as TurretUpgrade[]);
    }

    void SetPositionOnSpline(Turret turret)
    {
        SplinePositioner splinePositioner = turret.GetComponent<SplinePositioner>();
        splinePositioner.spline = spline;
        if (_turretList.Count == 0)
        {
            splinePositioner.followTarget = engine.GetComponent<SplineTracer>();
        }
        else
        {
            splinePositioner.followTarget = _turretList[^1].GetComponent<SplineTracer>();
        }
        splinePositioner.followTargetDistance = _turretList.Count == 0 ? engineSpacing : turretSpacing;
    }

    void Update()
    {
        var enginePos = engineFollower.result.percent;
        ProgressManager.Instance.UpdateProgress(enginePos);
    }
    
    private void BoostStartedHandler(float damageBoost, float fireRateBoost)
    {
        foreach (var turret in _turretList)
        {
            turret.ApplyBoost(damageBoost, fireRateBoost);
        }
    }

    private void BoostEndedHandler()
    {
        foreach (var turret in _turretList)
        {
            turret.RemoveBoost();
        }
    }
    
    private void RepairKitUsedHandler(float repairPercentage, Action<bool> callback)
    {
        if (Health >= MaxHealth) return;
        
        UpdateHealth(MaxHealth * repairPercentage);
        callback?.Invoke(true);
        
    }

    public int GetAvailablePower()
    {
        int availablePower = Power;
        foreach (var turret in _turretList)
        {
            availablePower -= turret.GetPowerCost();
        }
        return availablePower;
    }

    public bool IsMaxLevel(TrainPart part) => part.Level >= GetUpgradesFromType(part.TrainType).Length-1;

    public void UpgradeEngine()
    {
        UpgradeTrainPart(engine);
        EngineUpgrade[] upgrade = GetUpgradesFromType(engine.TrainType) as EngineUpgrade[];
        OnEngineUpgraded?.Invoke(upgrade[engine.Level], engine.Level+1<upgrade.Length?upgrade[engine.Level + 1]:null);
    }
    
    public void UpgradeTurret(Turret turret)
    {
        UpgradeTrainPart(turret);
        TurretUpgrade[] upgrade = GetUpgradesFromType(turret.TrainType) as TurretUpgrade[];
        OnTurretUpgraded?.Invoke(turret, upgrade[turret.Level], turret.Level+1<upgrade.Length?upgrade[turret.Level + 1]:null);

    }
    
    public void UpgradeTrainPart(TrainPart part)
    {
        if (IsMaxLevel(part))
        {
            Debug.LogWarning("max level");
            return;
        }
        
        Upgrade nextUpgrade = GetUpgradesFromType(part.TrainType)[part.Level + 1];
        int cost = nextUpgrade.ScrapCost;

        if (_playerInventory.TrySpendScarp(cost))
        {
            part.Upgrade();
        }
        else
        {
            Debug.LogWarning("not enough scrap");
        }
    }
}