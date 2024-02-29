using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Character
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField]
        private int _scarp;

        public int Scarp {
            get => _scarp;
            private set => _scarp = value;
        }
        
        public List<IConsumable> consumables;
        
        private void OnEnable()
        {
            LootBox.OnLootBoxDestroyed += OnLoot;
            IConsumable.OnConsumed += RemoveItem;
            Enemy.OnDeath += OnEnemyDeath;
        }

        private void Start()
        {
            consumables = new List<IConsumable>
            {
                new AirStrike(1),
                new RepairKit(1),
                new TrainBoost(1)
            };
            StartCoroutine(nameof(GenerateScrap));
        }
        
        private IEnumerator GenerateScrap()
        {
            while (gameObject.activeInHierarchy)
            {
                yield return new WaitForSeconds(1);
                AddScrap(10);
            }
        }

        private void OnLoot(IConsumable item)
        {
            AddItem(item);
        }
        
        public void OnUseItem(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                int index = (int)context.ReadValue<float>() - 1;
                
                if (consumables[index].amount > 0)
                {
                    consumables[index].Activate();
                }
            }
        }
        
        private void OnEnemyDeath()
        {
            AddScrap(1);
        }
        
        private void AddScrap(int amount)
        {
            Scarp += amount;
        }
        
        public bool TrySpendScarp(int amount)
        {
            if (Scarp >= amount)
            {
                Scarp -= amount;
                return true;
            }
            return false;
        }
        
        private void AddItem(IConsumable item)
        {
            IConsumable existingItem = consumables.Find(i => i.GetType() == item.GetType());
            if (existingItem != null)
            {
                existingItem.amount += item.amount;
            }
            else
            {
                consumables.Add(item);
            }
        }

        private void RemoveItem(IConsumable item)
        {
            IConsumable existingItem = consumables.Find(i => i.GetType() == item.GetType());
            if (existingItem == null)
            {
                return;
            }
            if (existingItem.amount >= 1)
            {
                existingItem.amount--;
            }
        }
    }
}