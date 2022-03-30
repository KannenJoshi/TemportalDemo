using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public abstract class EnemyAI : Entity
{
    /*[Header("Components")]
    [SerializeField] private Transform body;*/
    
    [Header("Movement")]
    //[SerializeField] private float moveSpeed = 10.0f;
    //[SerializeField] private float rotateSpeed = 1.0f;
    [SerializeField] private float rotateSpeed = 0.5f;
    [SerializeField] private float hoverHeight = 1.5f; // Hovers 1.5m off the ground, 0 if walks
    [SerializeField] private State startState = State.IDLE;
    private State currentState;
    
    [Header("Detection")]
    [SerializeField] private float sightRange = 20.0f;
    [SerializeField] private float attackRange = 15.0f;
    [SerializeField] [Range(0,360)] private float angle = 60;
    [SerializeField] private float alertTime = 5.0f; // Will try to chase for 5 seconds after losing player
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask portalsMask;
    [SerializeField] private float portalTrackThroughPortalTime = 1.0f;
    private bool canSeePlayer; // In range to see
    private bool canAttackPlayer; // In range to attack
    private Vector3 targetPosition;
    private float stoppingDistance;

    [Header("Combat")]
    [SerializeField] private float attackSpeed = 0.5f;

    private bool _attackFlag = true;
    
    private Vector3 lastSeenPlayerPos;
    private float lastSeenPlayerTime;
    
    private LayerMask entities;
    private NavMeshAgent agent;
    private bool gravityEnabled;
    private Firearm[] weapons;
    
    

    private enum State
    {
        IDLE,
        PATROL,
        CHASE,
        ATTACK
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        weapons = GetComponentsInChildren<Firearm>();
    }

    void Start()
    {
        currentState = startState;
        gravityEnabled = rb.useGravity;

        StartCoroutine(ScanForPlayer());
    }

    // Update is called once per frame
    protected override void UpdateBehaviour()
    {
        switch (currentState)
        {
            case State.IDLE:
                if (canSeePlayer)
                {
                    currentState = State.CHASE;
                    break;
                }
                transform.Rotate(0, rotateSpeed*Time.deltaTime, 0);
                break;
            case State.PATROL:
                if (canSeePlayer) currentState = State.CHASE;
                break;
            case State.CHASE:
                // Move and Rotate to Player
                if (canAttackPlayer)
                {
                    currentState = State.ATTACK;
                    ReadyWeapons(true);
                    break;
                }
                //if (!canSeePlayer) currentState = startState;
                //RotateToPlayer();
                MoveToPlayer();
                if (!canSeePlayer && Time.time - lastSeenPlayerTime > portalTrackThroughPortalTime)
                    CheckLostPlayer();
                break;
            case State.ATTACK:
                // Attack Player
                if (!canAttackPlayer)
                {
                    currentState = State.CHASE;
                    ReadyWeapons(false);
                    break;
                }
                PredictPlayerMovement();
                //MoveToPlayer();
                Fire();
                break;
        }
        
        //Debug.DrawLine(this.transform.position, this.transform.position + this.transform.forward, Color.green, 1, false);

    }

    public override void EnterPortal()
    {
        
    }

    public override void ExitPortal()
    {
        
    }

    protected virtual void MoveToPlayer()
    {
        var dirToPlayer = (lastSeenPlayerPos - transform.position).normalized;
        var distToPlayer = Vector3.Distance(transform.position, lastSeenPlayerPos);

        // Player is in sight currently and not in attack range
        if (canSeePlayer)
        {
            targetPosition = transform.position + dirToPlayer * distToPlayer;
            stoppingDistance = attackRange;
        }
        // Player not in sight but still is alerted
        else
        {
            // If player disappeared within set time
            if (Time.time - lastSeenPlayerTime <= portalTrackThroughPortalTime)
            {
                Vector3 directionToTarget = (lastSeenPlayerPos - transform.position).normalized;

                RaycastHit hit;
                Debug.DrawLine(transform.position, transform.position + directionToTarget * sightRange, Color.magenta, 0.5f);
                if (Physics.Raycast(transform.position, directionToTarget, out hit, sightRange, portalsMask,
                        QueryTriggerInteraction.Ignore))
                {
                    print("Lost Portal Raycast");
                    // If looking at Portal
                    if (hit.collider.tag.Equals("Portal"))
                    {
                        // Set dest to be slightly behind portal
                        lastSeenPlayerPos = hit.transform.position + 2f * hit.transform.forward;
                        print("Portal");
                        //stoppingDistance = 0.5f;
                    }
                }
                else if (hit.collider != null)
                {
                    print(hit.collider.name);
                }

                print(hit.transform.name);
            }

            stoppingDistance = 0f;
            targetPosition = lastSeenPlayerPos;
        }
        
        // Don't want to keep resetting destination if will be the same
        targetPosition += new Vector3(0, hoverHeight, 0);
        if (!agent.destination.Equals(targetPosition))
        {
            agent.destination = targetPosition;
            agent.stoppingDistance = stoppingDistance;
        }
    }

    protected virtual void RotateToPlayer()
    {
        //body.LookAt(new Vector3(0, lastSeenPlayerPos.y));
        //body.LookAt(lastSeenPlayerPos);
    }

    protected virtual void PredictPlayerMovement()
    {
        transform.LookAt(lastSeenPlayerPos);
    }

    protected virtual void CheckLostPlayer()
    {
        if (alertTime == 0) {
            currentState = startState;
            return; // If no Timeout, always hunt for player. Failsafe for forgetting to override this as empty
        }
        transform.Rotate(0, 360f / alertTime * Time.deltaTime, 0);
        if (Time.time - lastSeenPlayerTime >= alertTime + portalTrackThroughPortalTime && !canSeePlayer)
        {
            currentState = startState;
            agent.ResetPath();
        }
    }

    /*protected virtual void ApplyHoverForce()
    {
        var force = -Physics.gravity; // Exactly counters gravity
        var stablingForce = new Vector3(0, upThrust, 0);
        
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit);

        if (hit.distance > hoverHeight) force -= -stablingForce;
        else if (hit.distance < hoverHeight) force += -stablingForce;
        
        rb.AddForce(force, ForceMode.Acceleration);
    }*/
    
    protected virtual void Fire()
    {
        if (!_attackFlag) return;
        StartCoroutine(AttackWithDelay());
    }

    private void ReadyWeapons(bool b)
    {
        foreach (var weapon in weapons)
        {
            weapon.IsReady = b;
        }
    }

    private void Warp()
    {
        // Disappear 4D Shift Shader
        gameObject.SetActive(false);
    }

    private void CheckWarpEndPos()
    {
        
    }

    // https://www.youtube.com/watch?v=j1-OyLo77ss
    IEnumerator ScanForPlayer()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            yield return wait;
            Collider[] inRange = Physics.OverlapSphere(transform.position, sightRange, playerMask);
            
            canSeePlayer = false;
            canAttackPlayer = false;
            
            // Player in spherecast
            if (inRange.Length != 0)
            {
                Transform target = inRange[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                
                // Within FOV
                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    RaycastHit hit;
                    // If not blocked by Obstacle
                    if (Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget, obstacleMask, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.transform.tag.Equals("Player"))
                        {
                            canAttackPlayer = distanceToTarget <= attackRange;
                            canSeePlayer = true;
                            lastSeenPlayerPos = target.transform.position + new Vector3(0, 2, 0);
                            lastSeenPlayerTime = Time.time;
                        }
                    }
                }
            }
        }
    }
    
    IEnumerator AttackDelay()
    {
        _attackFlag = false;
        yield return new WaitForSeconds(1/attackSpeed);
        _attackFlag = true;
    }
    IEnumerator AttackWithDelay()
    {
        _attackFlag = false;
        foreach (var weapon in weapons)
        {
            if (weapon.IsReady && !weapon.IsReloading && !weapon.IsShooting)
            {
                weapon.IsShooting = true;
            }

            yield return new WaitForSeconds(1 / attackSpeed);
        }
        _attackFlag = true;
    }
}