using Character;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIOverlayController : MonoBehaviour
    {
        public TextMeshProUGUI scrapAmountText;
        public TextMeshProUGUI powerAmountText;
    
        private void OnEnable()
        {
            Inventory.OnScrapChange += OnScrapChange;
        }
    
        private void OnDisable()
        {
            Inventory.OnScrapChange -= OnScrapChange;
        }

        private void OnScrapChange(int scrap)
        {
            if (!scrapAmountText ) return;
            scrapAmountText.text = scrap.ToString();
        }
    
        public void OnPowerChange(int power)
        {
            if (!powerAmountText) return;
            powerAmountText.text = power.ToString();
        }
    }
}
