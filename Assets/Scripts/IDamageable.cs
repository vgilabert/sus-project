using UnityEngine;
using UnityEngine.Serialization;

public abstract class IDamageable : MonoBehaviour
{
    private StatusIndicator statusIndicator;
    
    [SerializeField] private float maxHealth;
    public float MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }
    protected float health;
    public float Health => health;
    protected bool dead;
    public bool Dead => dead;

    public static event System.Action OnDeath;

    protected virtual void Start()
    {
        health = maxHealth;
        statusIndicator = transform.GetComponentInChildren<StatusIndicator>();
        if (statusIndicator)
        {
            statusIndicator.SetMaxHealth(maxHealth);
        }
    }

    public virtual void TakeHit(float damage, Vector3 hitDirection = default)
    {
        TakeDamage(damage);
        if (statusIndicator)
        {
            statusIndicator.SetHealth(health);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath.Invoke();
        }
        Destroy(gameObject);
    }
}
