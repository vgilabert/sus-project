using System;
using UnityEngine;

public class TrainCar : MonoBehaviour, IDamageable
{
    private Train _train;

    private Turret _turret;
    
    private GameObject _slot;
    
    [SerializeField] private float _speed = 1f;
    
    private void Awake()
    {
        _train = GetComponentInParent<Train>();
        Debug.Log(_train);
        _slot = transform.Find("Slot").gameObject;
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
    
    public void AddTurret(Turret turret)
    {
        _turret = turret;
        _turret.transform.position = _slot.transform.position;
    }
}
