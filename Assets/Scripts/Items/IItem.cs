namespace Items
{
    public abstract class IItem
    {
        protected string Name => GetType().Name;
        public int Amount { get; set; }
        
        public IItem(int amount = 1)
        {
            Amount = amount;
        }
    }
}