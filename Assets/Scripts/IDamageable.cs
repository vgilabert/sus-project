using UnityEngine;

public abstract class IDamageable : MonoBehaviour
{
    private StatusIndicator statusIndicator;
    
    [SerializeField] private float maxHealth;
    
    [SerializeField] private float health;

    public float MaxHealth
    {
        get => maxHealth;
        protected set => SetMaxHealth(value);
    }

    public float Health
    {
        get => health;
        protected set => SetHealth(value);
    }
    
    protected bool dead;
    public bool Dead => dead;

    public static event System.Action OnDeath;

    protected virtual void Start()
    {
        Health = maxHealth;
        statusIndicator = transform.GetComponentInChildren<StatusIndicator>();
        if (statusIndicator)
        {
            statusIndicator.SetMaxHealth(maxHealth);
            statusIndicator.SetHealth(Health);
        }
    }

    public virtual void TakeHit(float damage, Vector3 hitDirection = default)
    {
        UpdateHealth(-damage);
    }

    protected virtual void UpdateHealth(float amount)
    {
        Health += amount;
        if (statusIndicator)
        {
            statusIndicator.SetHealth(Health);
        }
        if (Health <= 0 && !dead)
        {
            Die();
        }
    }
    
    private void SetMaxHealth(float amount)
    {
        maxHealth = amount;
        if (statusIndicator)
        {
            statusIndicator.SetMaxHealth(maxHealth);
        }
    }
    
    private void SetHealth(float amount)
    {
        health = amount;
        if (statusIndicator)
        {
            statusIndicator.SetHealth(health);
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
