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
        var lootType = (LootType)Random.Range(0, 4);
        Debug.Log(lootType);
        int amount;
        
        switch (lootType)
        {
            case LootType.Gravel:
                amount = Random.Range(10, 25);
                Inventory.Instance.AddItem(new Gravel(amount));
                break;
            case LootType.TrainBooster:
                amount = 1;
                Inventory.Instance.AddItem(new TrainBooster(amount));
                break;
            case LootType.AirStrike:
                amount = 1;
                Inventory.Instance.AddItem(new AirStrike(amount));
                break;
            case LootType.RepairKit:
                amount = Random.Range(1, 2);
                Inventory.Instance.AddItem(new RepairKit(amount));
                break;
        }
    }

    enum LootType
    {
        Gravel,
        TrainBooster,
        AirStrike,
        RepairKit
    }
}
