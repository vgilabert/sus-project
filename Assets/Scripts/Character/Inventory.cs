using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Character
{
    public class Inventory : MonoSingleton<Inventory>
    {
        [SerializeField]
        private int _scrap;

        public int Scrap {
            get => _scrap;
            private set => _scrap = value;
        }

        public Dictionary<ConsumableType, List<Consumable>> consumables = new()
        {
            {
                ConsumableType.AirStrike, new()
            },
            {
                ConsumableType.RepairKit, new()
            },
            {
                ConsumableType.TrainBoost, new()
            }
        };
        
        public static Action<int> OnScrapChange = delegate {  };
        public static Action<ConsumableType, int> OnConsumableChanged = delegate {  };
        
        private void OnEnable()
        {
            LootBox.OnLoot += OnLoot;
            Consumable.OnConsumed += OnConsumeHandler;
        }
        
        private void OnDisable()
        {
            LootBox.OnLoot -= OnLoot;
            Consumable.OnConsumed -= OnConsumeHandler;
        }

        private void Start()
        {   
            OnScrapChange?.Invoke(Scrap);
        }

        private void OnLoot(IItem item, int amount)
        {
            if (item is Scrap scrap)
            {
                AddScrap(amount);
            }
            else if (item is Consumable consumable)
            {
                AddConsumable(consumable);
            }
        }
        
        public void OnUseConsumable(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                int index = (int)context.ReadValue<float>() - 1;
                
                var type = (ConsumableType)index;
                if (consumables[type].Count > 0)
                {
                    consumables[type][^1].Activate();
                }
            }
        }
        
        public void AddScrap(int amount)
        {
            Scrap += amount;
            OnScrapChange?.Invoke(Scrap);
        }
        
        public bool TrySpendScarp(int amount)
        {
            if (Scrap >= amount)
            {
                AddScrap(-amount);
                return true;
            }
            return false;
        }
        
        private void OnConsumeHandler(Consumable consumable)
        {
            RemoveConsumable(consumable);
        }

        private void AddConsumable(Consumable consumable)
        {
            if (!consumables.ContainsKey(consumable.Type)) return;
            
            consumables[consumable.Type].Add(consumable);
            OnConsumableChanged?.Invoke(consumable.Type, consumables[consumable.Type].Count);
        }
        
        private void RemoveConsumable(Consumable consumable)
        {
            if (!consumables.ContainsKey(consumable.Type)) return;
            
            consumables[consumable.Type].Remove(consumable);
            OnConsumableChanged?.Invoke(consumable.Type, consumables[consumable.Type].Count);
        }
    }
}