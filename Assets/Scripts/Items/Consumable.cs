using System;
using UnityEngine;

namespace Items
{
    
    public class Consumable : IItem
    {
        private ItemFlow _itemFlow;
        
        private ConsumableType _type;
        public ConsumableType Type => _type;
        
        public static Action<Consumable> OnConsumed;
        
        public Consumable(ConsumableType type)
        {
            _type = type;
        }

        public void Activate()
        {
            InitItemFlow();
            if (_itemFlow != null)
            {
                _itemFlow.StartFlow(_success => OnConsumed?.Invoke(this));
            }
            else
            {
                Debug.LogWarning("ItemFlow not found");
            }
        }

        private void InitItemFlow()
        {
            switch (_type)
            {
                case ConsumableType.AirStrike:
                    _itemFlow = GameObject.FindFirstObjectByType<AirStrikeFlow>();
                    break;
                case ConsumableType.RepairKit:
                    _itemFlow = GameObject.FindFirstObjectByType<RepairKitFlow>();
                    break;
                case ConsumableType.TrainBoost:
                    _itemFlow = GameObject.FindFirstObjectByType<TrainBoostFlow>();
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