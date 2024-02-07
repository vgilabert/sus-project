using UnityEngine;
using Weapons;

public class TurretController : MonoBehaviour
{
    private GameObject turret;
    private Gun gun;
    private Cart cart;

    private Transform turretAnchor;
    
    private Transform Target { get; set; }
    
    public float Range { get; set; }

    public void SetControllerData(GameObject c, GameObject turretPrefab)
    {
        cart = c.GetComponent<Cart>();
        turretAnchor = transform.GetChild(0).transform;
        turret = Instantiate(turretPrefab, turretAnchor.position, Quaternion.identity, transform);
        gun = turret.GetComponent<Gun>();
    }

    void Update()
    {
        UpdateTarget();
        UpdateRotation();
        if (Target)
            gun.Shoot(Target);
    }
    
    private void UpdateTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Range);
        
        Collider nearestEnemy = null;
        float minDistance = Mathf.Infinity;
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.gameObject.CompareTag("Enemy"))
            {
                continue;
            }

            Vector3 pos = hitCollider.gameObject.transform.position;
            
            float distance = Vector3.Distance(transform.position, pos);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = hitCollider;
            }
        }

        if (!nearestEnemy)
        {
            Target = null;
        }
        else
        {
            Target = nearestEnemy.transform;
        }
    }


    protected void UpdateRotation()
    {
        if  (!Target) return;
        if (cart.cartType == CartType.Gatling)
        {
            turret.transform.LookAt(new Vector3(Target.position.x, turret.transform.position.y, Target.position.z));
        }
        else if (cart.cartType == CartType.Missile)
        {
            turret.transform.LookAt(Target);
        }
        
    }
}