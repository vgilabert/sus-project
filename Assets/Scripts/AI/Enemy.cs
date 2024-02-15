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
    Transform target;
    Material skinMaterial;
    
    PlayerStats player;

    public GameObject damageEffect;
    public GameObject deathEffect; 

    Color originalColour;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    protected override void Start()
    {
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColour = skinMaterial.color;

        OnDeath += RemoveAgent;

        currentState = State.Chasing;
        GetClosestTarget();

        myCollisionRadius = GetComponent<CapsuleCollider>().radius;

        avoidance = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<Avoidance>();
        StartCoroutine(UpdatePath());
    }

    void Update()
    {
        if (Time.time > nextAttackTime)
        {
            if (GetClosestTarget() < Mathf.Pow(attackDistanceThreshold + myCollisionRadius, 2))
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
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

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
        
        skinMaterial.color = originalColour;
        currentState = State.Chasing;
        pathfinder.enabled = true;

        player.TakeHit(1, new RaycastHit(), dirToTarget.normalized);
    }

    float GetClosestTarget()
    {
        float sqrDstToTarget = Mathf.Infinity;
        List<GameObject> targets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        foreach(GameObject t in targets) 
        { 
            if((t.transform.position - transform.position).sqrMagnitude< sqrDstToTarget)
            {
                sqrDstToTarget = (t.transform.position - transform.position).sqrMagnitude;
                target = t.transform;
            }
        }
        return sqrDstToTarget;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;

        while (target != null)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + attackDistanceThreshold / 2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    public override void TakeHit(float damage, RaycastHit hit, Vector3 hitDirection = default)
    {
        if (damageEffect)
        {
            Destroy(Instantiate(damageEffect.gameObject, hit.point, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, 10f);
        }
        base.TakeHit(damage, hit, hitDirection);
    }
    
    protected override void Die()
    {
        if (deathEffect)
        {
            Destroy(Instantiate(deathEffect.gameObject, transform.position, Quaternion.identity) as GameObject, 10f);
        }
        base.Die();
    }

    public void RemoveAgent()
    {
        Avoidance avoidance = GameObject.FindGameObjectWithTag("CrowdManager")?.GetComponent<Avoidance>();
        if(avoidance != null) { avoidance.RemoveAgent(pathfinder); }
    }
}