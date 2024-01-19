using UnityEngine;

public abstract class IDamageable : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;
    private float MaxHealth => maxHealth;

    public float Health { get; private set; }
        
        
    private bool IsDead { get; set; }

    public event System.Action OnDeath;
        
    protected virtual void Start()
    {
        Health = MaxHealth;
    }

    public void TakeHit(float damage, RaycastHit hit, Vector3 hitDirection = default)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        if (damage >= Health)
        {
            Die();
        }
        else
        {
            Health -= damage;
        }
    }
        
    protected virtual void Die()
    {
        Debug.Log("Dies");
        IsDead = true;
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}