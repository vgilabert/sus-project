using UnityEngine;

namespace Items
{
    public class TrainBoost : IConsumable
    {
        public TrainBoost(int amount = 1) : base(amount)
        {
            name = nameof(TrainBoost);
        }
        
        public override void Activate()
        {
            Debug.Log("train boost");
        }
    }
}
