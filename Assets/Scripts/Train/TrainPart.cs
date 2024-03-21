using Train.Upgrades;
using UnityEngine;

public class TrainPart : IDamageable
{
    protected TrainManager trainManager;
    
    public TrainType TrainType { get;  set; }

    public int Level { get; protected set; }
    
    protected Upgrade[] Upgrades { get; set; }
    
    protected override void Start()
    {
        base.Start();
        trainManager = transform.GetComponentInParent<TrainManager>();
        CrowdController.Instance.AddTarget(gameObject.GetComponent<IDamageable>());
    }
    
    protected virtual void ApplyUpgrade()
    { }
    
    public void Upgrade()
    {
        if (Level >= Upgrades.Length)
            return;
        
        Level++;
        ApplyUpgrade();
    }
    
    public override void TakeHit(float dmg, Vector3 hitDirection = default)
    {
        if (!trainManager)
            return;
        trainManager.TakeHit(dmg, hitDirection);
    }
    
    protected override void Die()
    {
        base.Die();
        CrowdController.Instance.RemoveTarget(this);
    }
}
