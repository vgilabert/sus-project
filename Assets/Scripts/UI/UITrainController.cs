using System;
using System.Collections.Generic;
using Character;
using TMPro;
using Train;
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
    private List<UIAddWagonButton> buttons;
    
    private void Awake()
    {
        buttons = new List<UIAddWagonButton>(transform.GetComponentsInChildren<UIAddWagonButton>());
        trainPartsLayout = transform.Find("Train Parts");
        wagonsLayout = trainPartsLayout.Find("Wagons");
        foreach (var button in buttons)
        {
            button.SetScrapCostText();
            button.SetPowerCostText();
        }
    }

    private void FixedUpdate()
    {
        UpdateWagons();
        UpdateButtons();
    }
    
    private void UpdateButtons()
    {
        foreach (var button in buttons)
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
    
    private void UpdateWagons()
    {
        foreach (Transform child in wagonsLayout)
        {
            Destroy(child.gameObject);
        }
        foreach (var wagon in trainManager.Wagons)
        {
            if (wagon is Missile)
            {
                Instantiate(rocketPreviewPrefab, wagonsLayout);
            }
            else if (wagon is Gatling)
            {
                Instantiate(gatlingPreviewPrefab, wagonsLayout);
            }
        }
    }
}
