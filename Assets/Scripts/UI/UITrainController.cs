using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Character;
using TMPro;
using Train;
using UI;
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
    }
    
    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(CustomUpdate());
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
    

    private void UpgradeWagon(int index)
    {
        var button = AddButtons[index];
        if (button.hasEnoughScrap && button.hasEnoughPower)
        {
            trainManager.UpgradeWagon(index);
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
    
    private void UpdateUpgradeButtons()
    {
        foreach (Transform child in wagonsLayout)
        {
            Destroy(child.gameObject);
        }
        foreach (var wagon in trainManager.Wagons)
        {
            UIWagonButton upgradeButton = null;
            if (wagon is Missile)
            {
                upgradeButton = Instantiate(rocketPreviewPrefab, wagonsLayout).GetComponentInChildren<UIWagonButton>();
            }
            else if (wagon is Gatling)
            {
                upgradeButton = Instantiate(gatlingPreviewPrefab, wagonsLayout).GetComponentInChildren<UIWagonButton>();
            }

            if (upgradeButton)
            {
                upgradeButton.ButtonScript.onClick.AddListener(() => UpgradeWagon(trainManager.Wagons.IndexOf(wagon)));
                upgradeButton.turretStat = wagon.GetNextUpgradeStats();
                UpgradeButtons.Add(upgradeButton);
            }
        }
        
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
