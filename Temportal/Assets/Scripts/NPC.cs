using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using Weapons;

public enum AIState
{
    IDLE,
    PATROL,
    CHASE,
    ATTACK
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public abstract class NPC : Entity
{
    [Header("AI State")]
    [SerializeField] private AIState initialState;
    [SerializeField] private AIState currentState;
    [SerializeField] private AIState previousState;

    [Header("Detection")]
    [SerializeField] private Transform head;
    [SerializeField] private GameObject target;
    [SerializeField] private LayerMask losLayer;
    [SerializeField] private float sightRange = 20.0f;
    [SerializeField] private float sightAngle = 120.0f;
    [SerializeField] private float attackRange = 10.0f;
    [SerializeField] private float alertTime = 5.0f;
    [SerializeField] private float thinkRate = 0.2f;
    [SerializeField]private float _lastThinkTime;
    [SerializeField]private Vector3 _lastSeenTargetPos;
    [SerializeField]private float _lastSeenTargetTime;
    [SerializeField]private bool _canSeeTarget;
    [SerializeField]private bool _canAttackTarget;
    [SerializeField]private bool _trackingTarget;

    [Header("Behaviour")]
    [SerializeField] private float idleRotate = 5.0f;
    [SerializeField] private List<Vector3> patrolPoints;
    private float _idleRotateProgress;
    private int _patrolPointIndex;
    private Vector3 _currentPatrolPoint;
    
    private NavMeshAgent agent;
    private List<Firearm> weapons;
    
    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        currentState = previousState = initialState;
        
        target = target == null ? GameObject.FindGameObjectWithTag("Player") : target;
        weapons = GetComponentsInChildren<Firearm>().ToList();

        if (patrolPoints.Count == 0)
        {
            _currentPatrolPoint = transform.position;
            patrolPoints.Add(RoundedPosition(_currentPatrolPoint));
        }
    }

    protected void ChangeState(AIState newState)
    {
        previousState = currentState;
        currentState = newState;
    }

    private static Vector3 RoundedPosition(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x),
                           Mathf.Round(position.y),
                           Mathf.Round(position.z));
    }

    private bool IsAgentAtDestination()
    {
        // https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
        // Distance was E-07 so using approximately compare remain and stop dist
        return (Mathf.Approximately(agent.remainingDistance, agent.stoppingDistance) || Mathf.Approximately(Vector3.Distance(transform.position, RoundedPosition(_currentPatrolPoint)),agent.stoppingDistance)) && (!agent.hasPath || Mathf.Approximately(agent.velocity.sqrMagnitude, 0f));
    }

    private void ScanForTarget()
    {
        var targetCollider = target.GetComponent<CapsuleCollider>();
        // Distance and Direction to Previous Position
        var distance = Vector3.Distance(transform.position, target.transform.position);
        var direction = transform.forward;
        var directionToPlayer = (target.transform.position + targetCollider.center - transform.position).normalized;

        _canSeeTarget = false;
        _canAttackTarget = false;
        
        // If out of sight range or not within the angle
        if (distance <= sightRange && Vector3.Angle(transform.forward, directionToPlayer) <= sightAngle / 2)
        {
            direction = directionToPlayer;
        }
        
        Debug.DrawRay(head.position, direction*sightRange, Color.green, thinkRate);

        if (Physics.Raycast(head.position, direction, out RaycastHit hit, sightRange, losLayer, QueryTriggerInteraction.Collide))
        {
            // If finds the Target
            if (hit.transform.CompareTag(target.tag))
            {
                _trackingTarget = true;
                _canSeeTarget = true;
                _canAttackTarget = distance <= attackRange;
                
                var c = (CapsuleCollider) hit.collider;
                _lastSeenTargetPos = hit.transform.position + c.center;
                _lastSeenTargetTime = Time.time;
            }
            // If target being tracked but cannot see directly and NPC looking at Portal
            else if (hit.transform.CompareTag("Portal") && _trackingTarget && !_canSeeTarget)
            {
                // Head into the portal
                _lastSeenTargetPos = hit.transform.position;
                // Without, might timeout before reaches portal
                //_lastSeenTargetTime = Time.time;
                _lastSeenTargetTime += Time.deltaTime;
                agent.stoppingDistance = 0.0f;
            }
        }
    }

    private Vector3 NextPatrolPoint()
    {
        _patrolPointIndex++;
        _patrolPointIndex %= patrolPoints.Count;
        return RoundedPosition( patrolPoints[_patrolPointIndex] );
    }

    protected void Idle()
    {
        _idleRotateProgress = 0;
        ChangeState(AIState.IDLE);
    }

    protected void Patrol()
    {
        agent.stoppingDistance = 0.0f;
        agent.SetDestination(RoundedPosition(_currentPatrolPoint));
        ChangeState(AIState.PATROL);
    }

    protected void Chase()
    {
        ChangeState(AIState.CHASE);
    }

    protected void Attack()
    {
        agent.stoppingDistance = attackRange;
        ChangeState(AIState.ATTACK);
    }

    /*
     * VIRTUALS
     */
    
    // TO BE EXECUTED EVERY `THINKRATE` SECONDS
    protected virtual void AIThink()
    {
        // Check if still tracking target
        if (Time.time - _lastSeenTargetTime > alertTime)
        {
            _trackingTarget = false;
        }
        
        switch (currentState)
        {
            // If can Attack
            case AIState.IDLE:
                if (_canAttackTarget) Attack();
                else if (_trackingTarget) Chase();
                else IdleThink();
                
                break;
            case AIState.PATROL:
                if (_canAttackTarget) Attack();
                else if (_trackingTarget) Chase();
                else PatrolThink();
                
                break;
            case AIState.CHASE:
                if (_canAttackTarget) Attack();
                else if (!_trackingTarget) Patrol();
                // HAVE AN ENEMY WHICH ADDS LAST SEEN TO PATROL POINTS, UP TO X MANY STORED
                else ChaseThink();
                
                break;
            case AIState.ATTACK:
                if (!_canAttackTarget) Chase();
                else AttackThink();
                
                break;
        }
    }
    
    // TO BE EXECUTED EVERY FRAME
    protected virtual void AIBehaviour()
    {
        switch (currentState)
        {
            case AIState.IDLE:
                IdleBehaviour();
                break;
            case AIState.PATROL:
                PatrolBehaviour();
                break;
            case AIState.CHASE:
                ChaseBehaviour();
                break;
            case AIState.ATTACK:
                AttackBehaviour();
                break;
        }
    }

    protected virtual void IdleThink()
    {
        if (!(_idleRotateProgress >= 360)) return;
        _idleRotateProgress = 0.0f;
        Patrol();
    }
    
    protected virtual void PatrolThink()
    {
        if (IsAgentAtDestination())
        {
            _currentPatrolPoint = NextPatrolPoint();
            Idle();
        }
    }

    protected virtual void ChaseThink()
    {
        agent.stoppingDistance = _canSeeTarget ? attackRange : 0.0f;
        agent.SetDestination(RoundedPosition(_lastSeenTargetPos));
    }

    protected virtual void AttackThink()
    {
        agent.SetDestination(_lastSeenTargetPos);
    }


    protected virtual void IdleBehaviour()
    {
        if (idleRotate == 0.0f) return;
        var rotation = 360.0f * Time.deltaTime / idleRotate;
        transform.Rotate(0, rotation, 0);
        _idleRotateProgress += rotation;
    }
    
    protected virtual void PatrolBehaviour()
    {
        
    }
    
    protected virtual void ChaseBehaviour()
    {
        
    }
    
    protected virtual void AttackBehaviour()
    {
        // Rotate to Player and Fire Weapons
        //transform.LookAt(target.transform);
        var pos = target.transform.position; //_lastSeenTargetPos;
        pos.y = transform.position.y;
        transform.LookAt(pos);

        //https://stackoverflow.com/questions/35861951/unity-navmeshagent-wont-face-the-target-object-when-reaches-stopping-distance
        /*var pos = _lastSeenTargetPos;
        pos.y = 0;
        var rot = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, agent.angularSpeed*Time.deltaTime);*/
    }
    

    /*
     * OVERRIDES
     */
    protected override void UpdateBehaviour()
    {
        // Scan for Player every `thinkRate` seconds
        if (Time.time - _lastThinkTime >= thinkRate)
        {
            ScanForTarget();
            AIThink();
            _lastThinkTime = Time.time;
        }

        AIBehaviour();

        var direction = transform.forward;
        var t = Time.deltaTime;
        var a = new Vector3(0, sightAngle/2,0);
        var angle = Quaternion.Euler( a);
        var angle2 = Quaternion.Euler( -a);
        Debug.DrawRay(head.position, direction, Color.red, t);
        Debug.DrawRay(head.position, angle*direction*sightRange, Color.white, t);
        Debug.DrawRay(head.position, angle2*direction*sightRange, Color.white, t);

    }

    public override void Teleport(Transform start, Transform end)
    {
        //agent.enabled = false;
        base.Teleport(start, end);
        //agent.enabled = true;
        
        // Minimum Velocity
        var vel = Vector3.Max(agent.velocity, 4 * agent.velocity.normalized);
        agent.velocity = end.TransformVector(Quaternion.Euler(0.0f, 180.0f, 0.0f) * start.InverseTransformVector(vel));
    }

    protected override void Die()
    {
        base.Die();
    }
}