using Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class ConsumablesController : MonoBehaviour
    {
        private Inventory _inventory;

        private void Awake()
        {
            _inventory = Inventory.Instance;
        }

        public void OnUseItem1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                UseItem(ItemType.RepairKit);
            }
        }
    
        public void OnUseItem2(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                UseItem(ItemType.TrainBooster);
            }
        }
    
        public void OnUseItem3(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                UseItem(ItemType.AirStrike);
            }
        }
    
        private void UseItem(ItemType itemType)
        {
            var item = _inventory.Items[itemType];
            if (item is IConsumable)
            {
                if (item.Amount > 0)
                {
                    IConsumable consumable = (IConsumable) item;
                    consumable.Consume();
                    _inventory.UpdateItem(itemType, -1);
                }
            }
        

        }
    }
}
