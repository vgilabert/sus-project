using UnityEngine;

namespace Items
{
    public class AirStrike : IConsumable
    {
        public AirStrike(int amount = 1) : base(amount)
        {
            name = nameof(AirStrike);
        }
        
        protected override void Init()
        {
            if (itemFlow == null)
            {
                itemFlow = GameObject.FindFirstObjectByType<AirStrikeFlow>();
            }
            
        }
    }
}
