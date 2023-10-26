using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    public LayerMask damageMask;
    float speed = 10;
    public float damage = 1;

    public GameObject impactEffect;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }


    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        Debug.Log(hit.collider.gameObject.name);
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            if (( damageMask & ( 1 << hit.collider.transform.gameObject.layer)) == 0)
                damage = 0;
            Debug.Log("here : " + damage + " / " + hit.collider.gameObject.GetType());
            damageableObject.TakeHit(damage, hit);
        }
        else
        {
            Destroy(Instantiate(impactEffect.gameObject, hit.point, Quaternion.FromToRotation(Vector3.forward, transform.forward)) as GameObject, 3f);
        }
        GameObject.Destroy(gameObject);
    }
}
