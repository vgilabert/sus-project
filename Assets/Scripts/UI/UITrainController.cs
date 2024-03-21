using System.Collections;
using System.Collections.Generic;
using Character;
using Train;
using Train.Upgrades;
using UI;
using UnityEngine;

public class UITrainController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private TrainManager trainManager;

    [Header("UI Elements"), Space(5)]
    
    [SerializeField] private UITrainPartButton addGatlingButtonPrefab;
    [SerializeField] private UITrainPartButton addRocketButtonPrefab;
    
    [SerializeField] private UITrainPartButton engineUpgradeButtonPrefab;
    [SerializeField] private UITrainPartButton gatlingUpgradeButtonPrefab;
    [SerializeField] private UITrainPartButton rocketUpgradeButtonPrefab;

    [SerializeField] private Color availaibleScrapTextColor;
    [SerializeField] private Color unavailaibleScrapTextColor;
    [SerializeField] private Color availaiblePowerTextColor;
    [SerializeField] private Color unavailaiblePowerTextColor;
    
    private Transform addTurretsLayout;
    private Transform trainPartsLayout;
    private Transform wagonsLayout;
    
    private UITrainPartButton engineUpgradeButton;
    private List<UITrainPartButton> addButtons = new();
    private List<UITrainPartButton> UpgradeButtons;
    
    private Dictionary<Turret, UITrainPartButton> TurretUILink = new();
    
    private void Awake()
    {
        UpgradeButtons = new List<UITrainPartButton>();
        trainPartsLayout = transform.Find("Train Parts");
        wagonsLayout = trainPartsLayout.Find("Wagons");
        addTurretsLayout = transform.Find("Add Turret Buttons");
        
        InitializeButtons();
    }

    private void OnEnable()
    {
        StartCoroutine(CustomUpdate());
        trainManager.OnTurretBuilt += OnTurretBuilt;
        trainManager.OnTurretUpgraded += OnTurretUpgraded;
        trainManager.OnEngineUpgraded += OnEngineUpgraded;
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
        trainManager.OnTurretBuilt -= OnTurretBuilt;
        trainManager.OnTurretUpgraded -= OnTurretUpgraded;
        trainManager.OnEngineUpgraded -= OnEngineUpgraded;
    }
    
    private void InitializeButtons()
    {
        engineUpgradeButton = Instantiate(engineUpgradeButtonPrefab, trainPartsLayout);
        engineUpgradeButton.transform.SetSiblingIndex(0);
        engineUpgradeButton.upgrade = trainManager.engineUpgrades.upgrades[1];
        engineUpgradeButton.ButtonScript.onClick.AddListener(UpgradeEngine);
        UpgradeButtons.Add(engineUpgradeButton);
        
        var addGatlingButton = Instantiate(addGatlingButtonPrefab, addTurretsLayout);
        addGatlingButton.upgrade = trainManager.gatlingUpgrades.upgrades[0];
        addGatlingButton.ButtonScript.onClick.AddListener(AddGatling);
        addButtons.Add(addGatlingButton);

        var addRocketButton = Instantiate(addRocketButtonPrefab, addTurretsLayout);
        addRocketButton.upgrade = trainManager.rocketUpgrades.upgrades[0];
        addRocketButton.ButtonScript.onClick.AddListener(AddRocket);
        addButtons.Add(addRocketButton);
    }
    
    private IEnumerator CustomUpdate()
    {
        float time = 0;
        while (true)
        {
            time += Time.unscaledDeltaTime;
            if (time > 0.5f)
            {
                UpdateAddButtons();
                UpdateUpgradeButtons();
                time = 0;
            }
            yield return null;
        }
    }
    
    public void AddGatling()
    {
        var button = addButtons[0];
        if (button.hasEnoughScrap && button.hasEnoughPower)
        {
            trainManager.BuyGatling();
        }
    }
    
    public void AddRocket()
    {
        var button = addButtons[1];
        if (button.hasEnoughScrap && button.hasEnoughPower)
        {
            trainManager.BuyRocket();
        }
    }
    
    public void UpgradeEngine()
    {
        var button = engineUpgradeButton;
        if (trainManager.IsMaxLevel(trainManager.engine))
        {
            return;
        }
        if (button.hasEnoughScrap && button.hasEnoughPower)
        {
            trainManager.UpgradeEngine();
            if (trainManager.IsMaxLevel(trainManager.engine))
            {
                button.SetMaxLevel();
            }
        }
    }
    
    private void UpgradeTurret(Turret turret)
    {
        var button = TurretUILink[turret];

        if (trainManager.IsMaxLevel(turret))
        {
            return;
        }
        
        if (button.hasEnoughScrap && button.hasEnoughPower)
        {
            trainManager.UpgradeTurret(turret);
            if (trainManager.IsMaxLevel(turret))
            {
                button.SetMaxLevel();
            }
        }
    }
    
    private void UpdateAddButtons()
    {
        foreach (var button in addButtons)
        {
            button.hasEnoughScrap = button.upgrade.ScrapCost <= inventory.Scrap;
            button.hasEnoughPower = button.upgrade.PowerCost <= trainManager.GetAvailablePower();
            button.EnableButton();

            button.SetScrapCostTextColor(button.hasEnoughScrap ? availaibleScrapTextColor : unavailaibleScrapTextColor);
            button.SetPowerCostTextColor(button.hasEnoughPower ? availaiblePowerTextColor : unavailaiblePowerTextColor);
            
            button.SetScrapCostText();
            button.SetPowerCostText();
        }
    }
    
    private void UpdateUpgradeButtons()
    {
        foreach (var button in UpgradeButtons)
        {
            if (button == null) continue;
            if (button.upgrade == null) continue;
            button.hasEnoughScrap = button.upgrade.ScrapCost <= inventory.Scrap;
            button.hasEnoughPower = button.upgrade.PowerCost <= trainManager.GetAvailablePower();
            button.EnableButton();
            button.SetScrapCostTextColor(button.hasEnoughScrap ? availaibleScrapTextColor : unavailaibleScrapTextColor);
            button.SetPowerCostTextColor(button.hasEnoughPower ? availaiblePowerTextColor : unavailaiblePowerTextColor);
            
            button.SetScrapCostText();
            button.SetPowerCostText();
        }
    }
    
    private void OnTurretUpgraded(Turret turret, TurretUpgrade stat, TurretUpgrade nextStat)
    {
        TurretUILink[turret].upgrade = nextStat;
    }

    private void OnEngineUpgraded(EngineUpgrade stat, EngineUpgrade nextStat)
    {
        engineUpgradeButton.upgrade = nextStat;
    }

    private void OnTurretBuilt(Turret turret, TurretUpgrade stat, TurretUpgrade nextStat)
    {
        UITrainPartButton trainPartButton = null;
        if (turret.TrainType == TrainType.Rocket)
        {
            trainPartButton = Instantiate(rocketUpgradeButtonPrefab, wagonsLayout).GetComponentInChildren<UITrainPartButton>();
        }
        else if (turret.TrainType == TrainType.Gatling)
        {
            trainPartButton = Instantiate(gatlingUpgradeButtonPrefab, wagonsLayout).GetComponentInChildren<UITrainPartButton>();
        }

        if (trainPartButton)
        {
            TurretUILink[turret] = trainPartButton;
            trainPartButton.ButtonScript.onClick.AddListener(() => UpgradeTurret(turret));
            UpgradeButtons.Add(trainPartButton);
        }

        trainPartButton.upgrade = nextStat;
    }
}
