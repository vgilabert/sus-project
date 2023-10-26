using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class TrainCar : MonoBehaviour, IDamageable
{
    private Train _train;

    private GameObject _slot;
    
    [SerializeField] private GameObject _turretPrefab;

    [SerializeField] private float _speed = 1f;
    
    private void Awake()
    {
        _train = GetComponentInParent<Train>();
        Debug.Log(_train);
        _slot = transform.Find("Slot").gameObject;
    }

    private void Start()
    {
        AddTurret(_turretPrefab);
    }

    void FixedUpdate()
    {
        transform.position += Vector3.forward * (Time.deltaTime * _speed);
    }

    public void TakeHit(float damage, RaycastHit hit, Vector3 hitDirection = default)
    {
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
