using UnityEngine;

namespace Items
{
    public class RepairKit : IConsumable
    {
        public RepairKit(int amount = 1) : base(amount)
        {
            name = nameof(RepairKit);
        }
        
        public override void Activate()
        {
            // TODO: Implement repair kit logic
            Debug.Log("repair kit");
            base.Activate();
        }
        
    }
}
