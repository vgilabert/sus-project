using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class TrainCar : MonoBehaviour, IDamageable
{
    private Train _train;

    private GameObject _slot;

    [SerializeField] private GameObject _hitEffect;
    [SerializeField] private GameObject _turretPrefab;

    private void Awake()
    {
        if(_hitEffect == null)
        {
            _hitEffect = new GameObject();
            Debug.LogWarning("Hit Effect not set in train car " + this.name);
        }

        _train = GetComponentInParent<Train>();
        Debug.Log(_train);
        _slot = transform.Find("Slot").gameObject;
    }

    private void Start()
    {
        AddTurret(_turretPrefab);
    }

    public void TakeHit(float damage, RaycastHit hit, Vector3 hitDirection = default)
    {
        if(hitDirection != default)
            Destroy(Instantiate(_hitEffect.gameObject, hit.point, Quaternion.FromToRotation(-Vector3.forward, hitDirection)), 3f);
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        _train.TakeDamage(damage);
    }
    
    public void AddTurret(GameObject turret)
    {
        if(!turret)
            return;
        Instantiate(turret, _slot.transform.position, Quaternion.identity, transform);
    }
}
