using UnityEngine;

namespace Items
{
    public class RepairKit : IConsumable
    {
        public RepairKit(int amount = 1) : base(amount)
        { }
        
        protected override void Consume()
        {
            Debug.Log("Consumed RepairKit");
        }
    }
}