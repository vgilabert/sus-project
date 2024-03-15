using Character;
using Items;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIConsumableInfo : MonoBehaviour
    {
        [SerializeField] private Color availaibleBackgroundColor;
        [SerializeField] private Color unavailaibleBackgroundColor;
        
        private UIConsumable[] _consumables;

        private void Awake()
        {
            _consumables = GetComponentsInChildren<UIConsumable>();
            foreach (var consumableUI in _consumables)
            {
                SetAvailable(consumableUI, false);
            }
        }
        
        private void OnEnable()
        {
            Inventory.OnConsumableChanged += UpdateConsumableInfo;
        }
        
        private void OnDisable()
        {
            Inventory.OnConsumableChanged -= UpdateConsumableInfo;
        }

        private void UpdateConsumableInfo(ConsumableType type, int amount)
        {
            foreach (var consumableUI in _consumables)
            {
                if (consumableUI.consumable == type)
                {
                    consumableUI.consumableAmountText.text = amount.ToString();
                    SetAvailable(consumableUI, amount != 0);

                }
            }
        }
        
        private void SetAvailable(UIConsumable consumableUI, bool available)
        {
            if (available)
            {
                consumableUI.GetComponent<Image>().color = availaibleBackgroundColor;
            }
            else
            {
                consumableUI.GetComponent<Image>().color = unavailaibleBackgroundColor;
                consumableUI.consumableAmountText.text = "0";
            }
        }
    }
}
