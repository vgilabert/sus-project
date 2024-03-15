using Items;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIConsumable : MonoBehaviour
    {
        public ConsumableType consumable;
        [HideInInspector]
        public TextMeshProUGUI consumableAmountText;
        
        private void Awake()
        {
            consumableAmountText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
    }
}
