using UnityEngine;

namespace Items
{
    public class RepairKit : IConsumable
    {
        public RepairKit(int amount = 1) : base(amount)
        {
            name = nameof(RepairKit);
        }
        
        protected override void Init()
        {
            if (itemFlow == null)
            {
                itemFlow = GameObject.FindFirstObjectByType<RepairKitFlow>();
            }
        }
        
    }
}
