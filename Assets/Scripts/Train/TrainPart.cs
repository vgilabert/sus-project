using UnityEngine;

public class TrainPart : IDamageable
{
    private TrainManager _trainManager;

    protected override void Start()
    {
        base.Start();
        _trainManager = transform.GetComponentInParent<TrainManager>();
        CrowdController.Instance.AddTarget(gameObject.GetComponent<IDamageable>());
    }

    public override void TakeHit(float dmg, Vector3 hitDirection = default)
    {
        if (!_trainManager)
            return;
        _trainManager.TakeHit(dmg, hitDirection);
    }
    
    protected override void Die()
    {
        base.Die();
        CrowdController.Instance.RemoveTarget(this);
    }
}
