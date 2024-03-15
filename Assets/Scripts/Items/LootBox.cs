using System;
using Items;
using UnityEngine;

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

    protected override void UpdateHealth(float damage)
    {
        base.UpdateHealth(-1);
    }

    private void GenerateLoot()
    {
        if (UnityEngine.Random.Range(0, 2) == 0)
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
        return UnityEngine.Random.Range(minScrap, maxScrap);
    }

    private Consumable GetRandomConsumable()
    {
        ConsumableType type = (ConsumableType)UnityEngine.Random.Range(0, 3);
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
