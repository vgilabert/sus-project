using UnityEngine;

namespace Items
{
    public class TrainBooster : IConsumable
    {
        public TrainBooster(int amount = 1) : base(amount)
        {
        }
        
        protected override void Consume()
        {
            Debug.Log("Consumed TrainBooster");
        }
    }
}