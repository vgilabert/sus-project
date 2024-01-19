using System.Collections.Generic;
using Items;

namespace Player
{
    public class Inventory
    {
        private List<IItem> Items { get; }

        private static Inventory _instance;

        private Inventory()
        {
            Items = new List<IItem>();
        }

        public static Inventory Instance => _instance ??= new Inventory();
        
        public void AddItem(IItem item)
        {
            if (Items.Contains(item))
            {
                Items[Items.IndexOf(item)].Amount += item.Amount;
            }
            else
            {
                Items.Add(item);
            }
        }
    }
}