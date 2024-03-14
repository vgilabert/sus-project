using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Character;
using TMPro;
using Train;
using Train.UpgradesStats;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITrainController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private TrainManager trainManager;
    
    [SerializeField] private GameObject gatlingPreviewPrefab;
    [SerializeField] private GameObject rocketPreviewPrefab;

    [SerializeField] private Color availaibleScrapTextColor;
    [SerializeField] private Color unavailaibleScrapTextColor;
    [SerializeField] private Color availaiblePowerTextColor;
    [SerializeField] private Color unavailaiblePowerTextColor;

    private Transform trainPartsLayout;
    private Transform wagonsLayout;
    private List<UIWagonButton> AddButtons;
    private List<UIWagonButton> UpgradeButtons;
    
    private void Awake()
    {
        AddButtons = new List<UIWagonButton>(transform.GetComponentsInChildren<UIWagonButton>());
        UpgradeButtons = new List<UIWagonButton>();
        trainPartsLayout = transform.Find("Train Parts");
        wagonsLayout = trainPartsLayout.Find("Wagons");
        foreach (var button in AddButtons)
        {
            button.SetScrapCostText();
            button.SetPowerCostText();
        }

        trainManager.OnWagonBuilded += OnWagonBuilded;
        trainManager.OnWagonUpgraded += OnWagonUpgraded;
        
    }

    private void OnWagonUpgraded(Wagon wagon, TurretStat stat, TurretStat nextStat)
    {
        WagonUILink[wagon].turretStat = nextStat;
    }

    private void OnWagonBuilded(Wagon wagon, TurretStat stat, TurretStat nextStat)
    {
        UIWagonButton upgradeButton = null;
        if (wagon.WagonType == WagonType.Rocket)
        {
            upgradeButton = Instantiate(rocketPreviewPrefab, wagonsLayout).GetComponentInChildren<UIWagonButton>();
        }
        else if (wagon.WagonType == WagonType.Gatling)
        {
            upgradeButton = Instantiate(gatlingPreviewPrefab, wagonsLayout).GetComponentInChildren<UIWagonButton>();
        }

        if (upgradeButton)
        {
            WagonUILink[wagon] = upgradeButton;
            upgradeButton.ButtonScript.onClick.AddListener(() => UpgradeWagon(wagon));
            UpgradeButtons.Add(upgradeButton);
        }

        upgradeButton.turretStat = nextStat;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(CustomUpdate());
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
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
        var button = AddButtons[0];
        if (button.hasEnoughScrap && button.hasEnoughPower)
        {
            trainManager.BuyGatling();
        }
    }
    
    public void AddRocket()
    {
        var button = AddButtons[1];
        if (button.hasEnoughScrap && button.hasEnoughPower)
        {
            trainManager.BuyRocket();
        }
    }
    
    private void UpgradeWagon(Wagon wagon)
    {
        var button = WagonUILink[wagon];
        if (button.hasEnoughScrap && button.hasEnoughPower)
        {
            trainManager.UpgradeWagon(wagon);
            if (trainManager.IsMaxLevel(wagon))
            {
                button.SetMaxLevel();
            }
        }
    }
    
    private void UpdateAddButtons()
    {
        foreach (var button in AddButtons)
        {
            button.hasEnoughScrap = button.turretStat.scrapCost <= inventory.Scrap;
            button.hasEnoughPower = button.turretStat.powerCost <= trainManager.GetAvailablePower();
            button.EnableButton();

            button.SetScrapCostTextColor(button.hasEnoughScrap ? availaibleScrapTextColor : unavailaibleScrapTextColor);
            button.SetPowerCostTextColor(button.hasEnoughPower ? availaiblePowerTextColor : unavailaiblePowerTextColor);
            
            button.SetScrapCostText();
            button.SetPowerCostText();
        }
    }

    private Dictionary<Wagon, UIWagonButton> WagonUILink = new();

    private void UpdateUpgradeButtons()
    {
        foreach (var button in UpgradeButtons)
        {
            if (button == null) continue;
            if (button.turretStat == null) continue;
            button.hasEnoughScrap = button.turretStat.scrapCost <= inventory.Scrap;
            button.hasEnoughPower = button.turretStat.powerCost <= trainManager.GetAvailablePower();
            button.EnableButton();
            button.SetScrapCostTextColor(button.hasEnoughScrap ? availaibleScrapTextColor : unavailaibleScrapTextColor);
            button.SetPowerCostTextColor(button.hasEnoughPower ? availaiblePowerTextColor : unavailaiblePowerTextColor);
            
            button.SetScrapCostText();
            button.SetPowerCostText();
        }
    }
}
