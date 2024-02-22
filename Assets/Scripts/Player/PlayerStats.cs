public class PlayerStats : IDamageable
{
    protected override void Start()
    {
        base.Start();
        CrowdController.Instance.AddTarget(gameObject.GetComponent<IDamageable>());
    }
    
    protected override void Die()
    {
        base.Die();
        CrowdController.Instance.RemoveTarget(this);
    }
}
