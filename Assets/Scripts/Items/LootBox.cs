using Items;
using Player;
using UnityEngine;
using Weapons;

public class LootBox : IDamageable
{
    protected override void Die()
    {
        base.Die();
        GenerateLoot();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(1);
    }

    private void GenerateLoot()
    {
        var lootType = (ItemType)Random.Range(0, 4);
        int amount = 0;
        
        switch (lootType)
        {
            case ItemType.Gravel:
                amount = Random.Range(10, 25);
                break;
            case ItemType.TrainBooster:
                amount = 1;
                break;
            case ItemType.AirStrike:
                amount = 1;
                break;
            case ItemType.RepairKit:
                amount = Random.Range(1, 2);
                break;
        }
        
        Inventory.Instance.UpdateItem(lootType, amount);
    }
}
