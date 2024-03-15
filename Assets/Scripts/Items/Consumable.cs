using System;
using UnityEngine;

namespace Items
{
    
    public class Consumable : IItem
    {
        private ConsumableFlow _consumableFlow;

        public ConsumableType Type { get; }
        
        public static Action<Consumable> OnConsumed;
        
        public Consumable(ConsumableType type)
        {
            Type = type;
        }

        public void Activate()
        {
            InitItemFlow();
            if (_consumableFlow != null)
            {
                Debug.Log("flow start");
                _consumableFlow.StartFlow(_success => OnConsumed?.Invoke(this));
            }
            else
            {
                Debug.LogWarning("ItemFlow not found");
            }
        }

        private void InitItemFlow()
        {
            switch (Type)
            {
                case ConsumableType.AirStrike:
                    _consumableFlow = GameObject.FindFirstObjectByType<AirStrikeFlow>();
                    break;
                case ConsumableType.RepairKit:
                    _consumableFlow = GameObject.FindFirstObjectByType<RepairKitFlow>();
                    break;
                case ConsumableType.TrainBoost:
                    _consumableFlow = GameObject.FindFirstObjectByType<TrainBoostFlow>();
                    break;
            }
        }
    }
    
    public enum ConsumableType
    {
        AirStrike,
        RepairKit,
        TrainBoost
    }
}