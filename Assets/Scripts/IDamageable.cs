using UnityEngine;

public interface IDamageable
{
    void TakeHit(float damage, RaycastHit hit, Vector3 hitDirection = default);

    void TakeDamage(float damage);

}