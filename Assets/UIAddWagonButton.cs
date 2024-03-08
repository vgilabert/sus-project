using System.Collections;
using System.Collections.Generic;
using TMPro;
using Train.UpgradesStats;
using UnityEngine;
using UnityEngine.UI;

public class UIAddWagonButton : MonoBehaviour
{
    public TurretStat turretStat;
    private TextMeshPro ScrapCostText;
    private TextMeshPro PowerCostText;

    public void EnableButton()
    {
        gameObject.GetComponent<Button>().interactable = true;
    }
    
    public void DisableButton()
    {
        gameObject.GetComponent<Button>().interactable = false;
    }
}
