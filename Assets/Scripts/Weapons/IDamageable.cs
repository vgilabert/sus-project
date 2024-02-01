using UnityEngine;

public abstract class IDamageable : MonoBehaviour
{

    public float startingHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }


    public void TakeHit(float damage, RaycastHit hit, Vector3 hitDirection = default)
    {
        // Do something
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(health);
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
        GameObject.Destroy(gameObject);
    }
}
