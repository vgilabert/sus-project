using System;
using Items;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootBox : IDamageable
{
    [SerializeField] private int minScrap;
    [SerializeField] private int maxScrap;
    
    public static Action<IItem, int> OnLoot = delegate {  };

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
        if (Random.Range(0, 2) == 0)
        {
            OnLoot?.Invoke(GetRandomConsumable(), 1);
        }
        else
        {
            OnLoot?.Invoke(new Scrap(), GetRandomNumber());
        }
    }

    private int GetRandomNumber()
    {
        return Random.Range(minScrap, maxScrap);
    }

    private Consumable GetRandomConsumable()
    {
        ConsumableType type = (ConsumableType)Random.Range(0, 3);
        return new Consumable(type);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Die();
        }
    }
}
