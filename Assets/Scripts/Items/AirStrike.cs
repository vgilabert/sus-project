using UnityEngine;

namespace Items
{
    public class AirStrike : IConsumable
    {
        public AirStrike(int amount = 1) : base(amount)
        { }
        
        protected override void Consume()
        {
            Debug.Log("Consumed AirStrike");
        }
    }
}