using System;
using System.Collections;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    private Gun _canon;
    
    private Vector3 _target;

    [SerializeField] private String TargetTag = "Player";
    
    [SerializeField] float Range = 10f;
    
    [SerializeField] bool drawDebugGizmos = true;
    
    private void Awake()
    {
        _canon = GetComponentInChildren<Gun>();
        if (TargetTag == null)
        {
            Debug.LogWarning("TargetTag should be set");
        }
    }

    private void FixedUpdate()
    {
        _target = GetNearestEnemy();
        if (_target != Vector3.zero)
        {
            _canon.transform.rotation = Quaternion.Slerp(_canon.transform.rotation, Quaternion.LookRotation(_target - _canon.transform.position), 0.3f);
        }
    }

    private IEnumerator Shoot()
    {
        while (_target != Vector3.zero)
        {
            _canon.Shoot();
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    private Vector3 GetNearestEnemy()
    {
        if (TargetTag == null)
        {
            return Vector3.zero;
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Range);
        
        Collider nearestEnemy = null;
        float minDistance = Mathf.Infinity;
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.gameObject.CompareTag(TargetTag))
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
            return Vector3.zero;
        }
        StartCoroutine(Shoot());
        return nearestEnemy.transform.position;
    }

    private void OnDrawGizmos()
    {
        if(!drawDebugGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}