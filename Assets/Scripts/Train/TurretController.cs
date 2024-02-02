using UnityEngine;
using Weapons;

public class TurretController : MonoBehaviour
{
    private GameObject turret;
    private Gun gun;
    private Cart cart;

    private Transform turretAnchor;
    
    private Transform Target { get; set; }

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
        gun.Shoot(Target);
    }
    
    private void UpdateTarget()
    {
        var targetPos = GameObject.Find("coucou")?.transform;
        if (cart.cartType == CartType.Gatling)
        {
            Target = targetPos;
        }
        else if (cart.cartType == CartType.Missile)
        {
            Target = targetPos;
        }
    }

    protected void UpdateRotation()
    {
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