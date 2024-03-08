using System;
using System.Collections.Generic;
using Character;
using Train;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITrainController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private TrainManager trainManager;
    
    [SerializeField] private GameObject engineUIPrefab;
    [SerializeField] private GameObject gatlingUIPrefab;
    [SerializeField] private GameObject rocketUIPrefab;

    [SerializeField] private Color availaibleScrapTextColor;
    [SerializeField] private Color unavailaibleScrapTextColor;
    [SerializeField] private Color availaiblePowerTextColor;
    [SerializeField] private Color unavailaiblePowerTextColor;

    private GameObject trainPartsLayout;
    private List<UIAddWagonButton> buttons;
    
    private void Awake()
    {
        buttons = new List<UIAddWagonButton>(transform.GetComponentsInChildren<UIAddWagonButton>());
        trainPartsLayout = transform.Find("Train Parts").gameObject;
        foreach (var button in buttons)
        {
            button.SetScrapCostText();
            button.SetPowerCostText();
        }
    }

    private void FixedUpdate()
    {
        UpdateTrainParts();
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
        }
    }
    
    private void UpdateTrainParts()
    {
        foreach (Transform child in trainPartsLayout.transform)
        {
            Destroy(child.gameObject);
        }
        Instantiate(engineUIPrefab, trainPartsLayout.transform);
        trainPartsLayout.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (trainManager.Wagons.Count + 1) * 200 + 100);
        foreach (var trainPart in trainManager.Wagons)
        {
            if (trainPart is Missile)
            {
                Instantiate(rocketUIPrefab, trainPartsLayout.transform);
            }
            else if (trainPart is Gatling)
            {
                Instantiate(gatlingUIPrefab, trainPartsLayout.transform);
            }
        }
    }
}
