using UnityEngine;

public class TrainPart : IDamageable
{
    protected TrainManager trainManager;

    protected override void Start()
    {
        base.Start();
        trainManager = transform.GetComponentInParent<TrainManager>();
        CrowdController.Instance.AddTarget(gameObject.GetComponent<IDamageable>());
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
