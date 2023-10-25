using System;
using UnityEngine;

public class TrainCar : MonoBehaviour, IDamageable
{
    private Train _train;

    private Turret _turret;
    
    private GameObject _slot;
    
    private void Awake()
    {
        _train = GetComponentInParent<Train>();
        _slot = transform.Find("Slot").gameObject;
    }

    void FixedUpdate()
    {
        transform.position += Vector3.forward * (Time.deltaTime * 10);
    }

    public void TakeHit(float damage, RaycastHit hit)
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
