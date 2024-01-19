namespace Items
{
    public abstract class IConsumable : IItem
    {
        protected IConsumable(int amount = 1) : base(amount) {}
        protected abstract void Consume();
    }
}
