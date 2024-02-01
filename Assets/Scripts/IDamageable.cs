using UnityEngine;
using UnityEngine.Serialization;

public abstract class IDamageable : MonoBehaviour
{

    [SerializeField] private float maxHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = maxHealth;
    }


    public virtual void TakeHit(float damage, RaycastHit hit, Vector3 hitDirection = default)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
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
