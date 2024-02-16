using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField]
        private int scrap;

        public int Scrap {
            get => scrap;
            private set => scrap = value;
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
                addScrap(10);
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
            addScrap(1);
        }
        
        private void addScrap(int amount)
        {
            Scrap += amount;
        }
        
        private void removeScrap(int amount)
        {
            if (Scrap - amount < 0)
            {
                Scrap = 0;
            }
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