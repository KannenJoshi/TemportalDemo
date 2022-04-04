using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;

public abstract class Entity : PortalTraveller
{
    [Header("Health")]
    [SerializeField] private int hpMax = 100;
    [SerializeField] private float hp = 100.0f;
    [SerializeField] private bool regenerate = true;
    [SerializeField] private float healAfterDamageDelay = 3.0f;
    [SerializeField] private float healTime = 0.1f; //(hp)s-1
    [SerializeField] private int healAmount = 5;
    [SerializeField] private float resistanceMultiplier = 1.0f; // e.g. takes 0.8x damage if set to 0.8

    [Header("Teleportation Settings")]
    [SerializeField] private float teleportOffsetPosition = 0.1f;
    [SerializeField] private float rotateXZDelay = 0.1f;
    
    private float _lastHit;
    private float _lastHeal;
    private float _rotationProgress;
    private Quaternion _rotationStart;
    private Quaternion _rotationEnd;
    private bool _rotationCorrectFlag = false;

    protected override void Awake()
    {
        base.Awake();
        hp = hpMax;
        _lastHit = Time.time;
        _lastHeal = Time.time;
    }

    void Update()
    {
        if (hp <= 0) Die();

        CorrectRotation();
        UpdateBehaviour();
        
        Heal();
    }

    public void ApplyDamage(int damage)
    {
        hp = Mathf.Max(hp - (damage * resistanceMultiplier), 0.0f);
        
        _lastHit = Time.time;
        print($"{name} : currentHp {hp}");
    }
    
    
    /*
     * OVERRIDES
     */
    public override void EnterPortal()
    {
             
    }

    public override void ExitPortal()
    {
        //_rotationCorrectFlag = true;
    }
    
    public override void Teleport(Transform start, Transform end)
    {
        transform.position = end.TransformPoint(Quaternion.Euler(0.0f, 180.0f, 0.0f) * start.InverseTransformPoint(transform.position));
        transform.position -= end.forward * teleportOffsetPosition;
        
        var rot = end.rotation * (Quaternion.Euler(0.0f, 180.0f, 0.0f) * Quaternion.Inverse(start.rotation) * transform.rotation);
        _rotationStart = rot;
        _rotationEnd = Quaternion.Euler(new Vector3(0, rot.eulerAngles.y, 0));;

        if (start.up.Equals(Vector3.up) && end.up.Equals(Vector3.up))
        {
            transform.rotation = _rotationEnd;
        }
        else
        {
            transform.rotation = rot;
            StartCoroutine(CorrectRotationDelay());
        }

        rb.velocity = end.TransformVector(Quaternion.Euler(0.0f, 180.0f, 0.0f) * start.InverseTransformVector(rb.velocity));
        //rb.velocity = Vector3.Max(rb.velocity, 50 * rb.velocity.normalized);

        Physics.SyncTransforms();
    }
    
    
    /*
     * VIRTUALS
     */
    protected virtual void UpdateBehaviour()
    {
        
    }
    
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
    
    protected virtual void Heal()
    {
        if (regenerate && hp < hpMax && Time.time > _lastHit + healAfterDamageDelay)
        {
            if (Time.time > _lastHeal + healTime)
            {
                hp += healAmount * healTime;//(Time.time - _lastHeal)*healTime;
                hp = Mathf.Min(hp, hpMax);
                
                _lastHeal = Time.time;
            }
        }
    }

    protected virtual void CorrectRotation()
    {
        if (_rotationCorrectFlag)
        {
            transform.rotation = 
                Quaternion.Slerp(_rotationStart, _rotationEnd, _rotationProgress += 5.0f * Time.deltaTime);
            
            if (_rotationProgress >= 1.0f)
            {
                _rotationProgress = 0.0f;
                _rotationCorrectFlag = false;
                //transform.rotation = _rotationEnd;
                //transform.rotation = Quaternion.Euler(_rotationEnd.eulerAngles.x, transform.eulerAngles.y, _rotationEnd.eulerAngles.z);
            }
        }
    }

    public virtual void ApplyRecoilTorque(float torque)
    {
        //rb.AddRelativeTorque(-torque * rb.mass, 0, 0, ForceMode.Impulse);
    }

    /*
     * GETTERS AND SETTERS
     */
    public int HpMax => hpMax;
    public float Hp => hp;

    /*
     * COROUTINES
     */
    private IEnumerator CorrectRotationDelay()
    {
        yield return new WaitForSecondsRealtime(rotateXZDelay);
        _rotationCorrectFlag = true;
    }
}
