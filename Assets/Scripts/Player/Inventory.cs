using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Player
{
    public class Inventory
    {
        public Dictionary<ItemType, IItem> Items { get; }

        private static Inventory _instance;

        private Inventory()
        {
            Items = new Dictionary<ItemType, IItem>
            {
                {ItemType.Gravel, new Gravel(0)},
                {ItemType.TrainBooster, new TrainBooster(0)},
                {ItemType.AirStrike, new AirStrike(0)},
                {ItemType.RepairKit, new RepairKit(0)}
            };
        }

        public static Inventory Instance => _instance ??= new Inventory();
        
        public void UpdateItem(ItemType itemType, int amount)
        {
            if (amount < 0 && Items[itemType].Amount < 0)
            {
                return;
            }

            if (amount > 0)
            {
                Debug.Log(itemType + " + " + amount);
            }

            switch (itemType)
            {
                case ItemType.Gravel:
                    Items[ItemType.Gravel].Amount += amount;
                    break;
                case ItemType.RepairKit:
                    Items[ItemType.RepairKit].Amount += amount;
                    break;
                case ItemType.TrainBooster:
                    Items[ItemType.TrainBooster].Amount += amount;
                    break;
                case ItemType.AirStrike:
                    Items[ItemType.AirStrike].Amount += amount;
                    break;
            }
        }
    }
}