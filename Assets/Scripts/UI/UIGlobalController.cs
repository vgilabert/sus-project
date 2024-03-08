using System;
using System.Collections.Generic;
using Character;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIGlobalController : MonoBehaviour
{
    [SerializeField]
    private GameObject uiTrain;

    public static Action<bool> setGamePause = delegate {  };
    
    public TextMeshProUGUI scrapText;
    public TextMeshProUGUI powerAmountText;

    private void OnEnable()
    {
        Inventory.OnScrapChange += OnScrapChange;
    }

    public void OnScrapChange(int scrap)
    {
        if (scrapText == null) return;
        scrapText.text = scrap.ToString();
    }

    public void SetPowerAmount(int power)
    {
        powerAmountText.text = power.ToString();
    }

    public void OnTrainMenuOpen(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!uiTrain.activeSelf) {
                setGamePause?.Invoke(true);
                uiTrain.SetActive(true);
            }
            else
            {
                setGamePause?.Invoke(false);
                uiTrain.SetActive(false);
            }
            
        }
    }
}
