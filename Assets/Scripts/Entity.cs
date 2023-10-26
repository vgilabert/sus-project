using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{

    public float startingHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath;


    [Header("Optional: ")]
    [SerializeField]
    private StatusIndicator statusIndicator;

    protected virtual void Start()
    {
        health = startingHealth;
        if(statusIndicator)
            statusIndicator.setMaxHealth(startingHealth);
    }

    public virtual void TakeHit(float damage, RaycastHit hit)
    {
        // Do some stuff here with hit var
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(statusIndicator)
            statusIndicator.SetHealth(health);
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    protected void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        Destroy(gameObject);
    }
}