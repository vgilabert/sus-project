using UnityEngine;

namespace Items
{
    public class TrainBoost : IConsumable
    {
        public TrainBoost(int amount = 1) : base(amount)
        {
            name = nameof(TrainBoost);
        }
        
        protected override void Init()
        {
            if (itemFlow == null)
            {
                itemFlow = GameObject.FindFirstObjectByType<TrainBoostFlow>();
            }
        }
    }
}
