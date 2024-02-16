using UnityEngine;

public class TrainPart : IDamageable
{
    private TrainManager _trainManager;

    protected override void Start()
    {
        base.Start();
        _trainManager = transform.GetComponentInParent<TrainManager>();
    }

    public override void TakeHit(float dmg, RaycastHit hit, Vector3 hitDirection = default)
    {
        if (!_trainManager)
            return;
        _trainManager.TakeHit(dmg, hit, hitDirection);
    }
}
