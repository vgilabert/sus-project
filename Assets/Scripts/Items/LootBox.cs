using System;
using Items;
using UnityEngine;

public class LootBox : IDamageable
{
    public static Action<IConsumable> OnLootBoxDestroyed = delegate {  };
    private IConsumable itemToLoot;
    
    protected override void Start()
    {
        base.Start();
        GenerateLoot();
    }
    
    protected override void Die()
    {
        base.Die();
        OnLootBoxDestroyed.Invoke(itemToLoot);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(1);
    }

    private void GenerateLoot()
    {
        itemToLoot = new IConsumable[] {new AirStrike(), new RepairKit(), new TrainBoost()}[UnityEngine.Random.Range(0, 3)];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Die();
        }
    }
}
