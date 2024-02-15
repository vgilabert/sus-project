using UnityEngine;
using UnityEngine.Serialization;

public abstract class IDamageable : MonoBehaviour
{
    private StatusIndicator statusIndicator;
    
    [SerializeField] private float maxHealth;
    protected float health;
    protected bool dead;

    public static event System.Action OnDeath;

    protected virtual void Start()
    {
        statusIndicator = transform.GetComponentInChildren<StatusIndicator>();
        if (statusIndicator)
        {
            statusIndicator.SetMaxHealth(maxHealth);
        }

        health = maxHealth;
    }

    public virtual void TakeHit(float damage, RaycastHit hit, Vector3 hitDirection = default)
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
            OnDeath();
        }
        Destroy(gameObject);
    }
}
