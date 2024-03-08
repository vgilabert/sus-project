using System;
using System.Collections.Generic;
using Character;
using Train;
using UnityEngine;
using UnityEngine.UI;

public class UITrainController : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private TrainManager trainManager;
    
    [SerializeField] private GameObject engineUIPrefab;
    [SerializeField] private GameObject gatlingUIPrefab;
    [SerializeField] private GameObject rocketUIPrefab;

    private GameObject trainPartsLayout;
    private List<UIAddWagonButton> buttons;
    
    private void Awake()
    {
        buttons = new List<UIAddWagonButton>(transform.GetComponentsInChildren<UIAddWagonButton>());
        trainPartsLayout = transform.Find("Train Parts").gameObject;
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
            if (button.turretStat.cost <= inventory.Scrap)
            {
                button.EnableButton();
            }
            else
            {
                button.DisableButton();
            }
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
