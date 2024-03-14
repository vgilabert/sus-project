using System;
using TMPro;
using Train.UpgradesStats;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIWagonButton : MonoBehaviour
    {
        public TurretStat turretStat;
        [SerializeField] private TextMeshProUGUI ScrapCostText;
        [SerializeField] private TextMeshProUGUI PowerCostText;

        public bool hasEnoughScrap;
        public bool hasEnoughPower;

        public Button ButtonScript { get; private set; }
    
        private void Awake()
        {
            ButtonScript = gameObject.GetComponent<Button>();
        }

        public void SetMaxLevel()
        {
            ScrapCostText.gameObject.SetActive(false);
            PowerCostText.gameObject.SetActive(false);
            ButtonScript.interactable = false;
        }
        public void SetScrapCostText()
        {
            ScrapCostText.text = turretStat.scrapCost.ToString();
        }

        public void SetPowerCostText()
        {
            PowerCostText.text = turretStat.powerCost.ToString();
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
            if (!ButtonScript) return;
            ButtonScript.interactable = hasEnoughScrap && hasEnoughPower;
        }
    
    }
}
