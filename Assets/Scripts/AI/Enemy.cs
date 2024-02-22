using NavMeshAvoidance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : IDamageable
{ 
    public enum State { Idle, Chasing, Attacking };
    State currentState;
    Avoidance avoidance;    

    NavMeshAgent pathfinder;
    Vector3 target;
    int targetIndex;
    Material skinMaterial;
    

    public GameObject damageEffect;
    public GameObject deathEffect; 

    Color originalColour;

    public float attackDistanceThreshold = 2f;
    float timeBetweenAttacks = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    protected override void Start()
    {
        base.Start();
        CrowdController.Instance.AddAgent(this);
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponentInChildren<Renderer>().material;
        originalColour = skinMaterial.color;

        OnDeath += RemoveAgent;

        currentState = State.Chasing;

        myCollisionRadius = 1;

        avoidance = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<Avoidance>();
        StartCoroutine(UpdatePath());
    }

    void Update()
    {
        if (Time.time > nextAttackTime)
        {
            var distanceToTarget = (target - transform.position).sqrMagnitude;
            if (distanceToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius, 2))
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target - transform.position).normalized;
        Vector3 attackPosition = target - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;
        
        while (percent <= 1)
        {
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }
        
        CrowdController.Instance.GetTarget(targetIndex).TakeHit(1);
        
        skinMaterial.color = originalColour;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    public void SetNearestTargetPositionAndIndex(Vector3 position, int index)
    {
        target = position;
        targetIndex = index;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;

        while (target != null)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target - transform.position).normalized;
                Vector3 targetPosition = target - dirToTarget * (myCollisionRadius + attackDistanceThreshold / 2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    public override void TakeHit(float damage, Vector3 hitDirection = default)
    {
        if (damageEffect)
        {
            Destroy(Instantiate(damageEffect.gameObject, transform.position, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, 10f);
        }
        base.TakeHit(damage, hitDirection);
    }
    
    protected override void Die()
    {
        if (deathEffect)
        {
            Destroy(Instantiate(deathEffect.gameObject, transform.position, Quaternion.identity), 1.5f);
        }
        CrowdController.Instance.RemoveAgent(this);
        base.Die();
    }

    public void RemoveAgent()
    {
        Avoidance avoidance = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<Avoidance>();
        if(avoidance != null) { avoidance.RemoveAgent(pathfinder); }
    }
}