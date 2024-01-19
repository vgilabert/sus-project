using UnityEngine;

namespace Items
{
    public class RepairKit : IConsumable
    {
        public RepairKit(int amount = 1) : base(amount)
        { }
        
        public override void Consume()
        {
            Debug.Log("Consumed RepairKit");
        }
    }
}