using TMPro;
using Train.Upgrades;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UITrainPartButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scrapCostText;
        [SerializeField] private TextMeshProUGUI powerCostText;

        public Upgrade upgrade;
        public bool hasEnoughScrap;
        public bool hasEnoughPower;

        public Button ButtonScript { get; private set; }
    
        private void Awake()
        {
            ButtonScript = gameObject.GetComponent<Button>();
        }

        public void SetMaxLevel()
        {
            scrapCostText.gameObject.SetActive(false);
            powerCostText.gameObject.SetActive(false);
            ButtonScript.interactable = false;
        }
        public void SetScrapCostText()
        {
            scrapCostText.text = upgrade.ScrapCost.ToString();
        }

        public void SetPowerCostText()
        {
            powerCostText.text = upgrade.PowerCost.ToString();
        }

        public void SetPowerCostTextColor(Color c)
        {
            powerCostText.color = c;
        }

        public void SetScrapCostTextColor(Color c)
        {
            scrapCostText.color = c;
        }

        public void EnableButton()
        {
            if (!ButtonScript) return;
            ButtonScript.interactable = hasEnoughScrap && hasEnoughPower;
        }
    
    }
}
