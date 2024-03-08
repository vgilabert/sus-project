using System.Collections;
using System.Collections.Generic;
using TMPro;
using Train.UpgradesStats;
using UnityEngine;
using UnityEngine.UI;

public class UIAddWagonButton : MonoBehaviour
{
    public TurretStat turretStat;
    [SerializeField] private TextMeshProUGUI ScrapCostText;
    [SerializeField] private TextMeshProUGUI PowerCostText;

    public bool hasEnoughScrap;
    public bool hasEnoughPower;
    public void SetScrapCostText()
    {
        ScrapCostText.text = turretStat.scrapCost.ToString();
    }

    public void SetPowerCostText()
    {
        PowerCostText.text = turretStat.powerCost.ToString();
        Debug.Log("PowerCostText: " + turretStat.powerCost);
    }

    public void SetPowerCostTextColor(Color c)
    {
        PowerCostText.color = c;
    }

    public void SetScrapCostTextColor(Color c)
    {
        ScrapCostText.color = c;
    }

    public void EnableButton()
    {
        gameObject.GetComponent<Button>().interactable = hasEnoughScrap && hasEnoughPower;
    }
    
}
