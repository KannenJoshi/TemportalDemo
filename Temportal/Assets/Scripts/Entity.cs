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
    
    private float _lastHit;
    private float _lastHeal;
    private float _rotationProgress;
    private Quaternion _rotationStart;
    private Quaternion _rotationEnd;
    private bool _rotationCorrectFlag = false;

    void Awake()
    {
        hp = hpMax;
        _lastHit = Time.time;
        _lastHeal = Time.time;
    }

    void Update()
    {
        if (hp <= 0) Die();
        
        UpdateBehaviour();
        
        Heal();
    }

    protected void Heal()
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

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void UpdateBehaviour()
    {
        if (_rotationCorrectFlag)
        {
            //transform.rotation = 
                Quaternion.Lerp(_rotationStart, _rotationEnd, _rotationProgress += 15.0f * Time.deltaTime);
            if (_rotationProgress >= 1.0f)
            {
                _rotationProgress = 0.0f;
                _rotationCorrectFlag = false;
                transform.rotation = _rotationEnd;
            }
        }
    }

    public override void Teleport(Transform start, Transform end)
    {
        transform.position = end.TransformPoint(Quaternion.Euler(0.0f, 180.0f, 0.0f) * start.InverseTransformPoint(transform.position));

        _rotationStart = transform.rotation;
        var rot = end.rotation * (Quaternion.Euler(0.0f, 180.0f, 0.0f) * Quaternion.Inverse(start.rotation) * _rotationStart);
        _rotationEnd = Quaternion.Euler(new Vector3(0, rot.eulerAngles.y, 0));
        //if (Mathf.Abs(rb.velocity.y) <= 0.1f)
        /*if (Mathf.Abs(rb.velocity.y) <= 0.1f)
            transform.rotation = _rotationEnd;
        else
            StartCoroutine(correctRotation());*/
        
        rb.velocity = end.TransformVector(Quaternion.Euler(0.0f, 180.0f, 0.0f) * start.InverseTransformVector(rb.velocity));
        if (start.up.Equals(Vector3.up) && end.up.Equals(Vector3.up))
            transform.rotation = _rotationEnd;
        else
            StartCoroutine(CorrectRotation());
        
        Physics.SyncTransforms();
        
        
    }

    public void ApplyDamage(int damage)
    {
        hp = Mathf.Max(hp - (damage * resistanceMultiplier), 0.0f);
        
        _lastHit = Time.time;
        print($"{name} : currentHp {hp}");
    }

    public virtual void ApplyRecoilTorque(float torque)
    {
        //rb.AddRelativeTorque(-torque * rb.mass, 0, 0, ForceMode.Impulse);
    }

    public int HpMax
    {
        get => hpMax;
    }

    public float Hp
    {
        get => hp;
    }

    private IEnumerator correctRotation()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        //yield return new WaitForFixedUpdate();
        //_rotationCorrectFlag = true;
        transform.rotation = _rotationEnd;
    }
}
