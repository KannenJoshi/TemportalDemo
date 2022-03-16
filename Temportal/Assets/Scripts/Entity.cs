using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : PortalTraveller
{
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

    // Update is called once per frame
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
            print(_rotationProgress);
            transform.rotation = Quaternion.Slerp(_rotationStart, _rotationEnd, _rotationProgress += 5.0f * Time.deltaTime);
            if (_rotationProgress >= 1.0f)
            {
                _rotationProgress = 0.0f;
                _rotationCorrectFlag = false;
            }
        }
    }

    public override void Teleport(Transform start, Transform end)
    {
        base.Teleport(start, end);
        _rotationStart = transform.rotation;
        //_rotationEnd = Quaternion.Euler(0.0f, 180.0f, 0.0f) * end.rotation * Quaternion.Inverse(start.rotation) * transform.rotation;
        _rotationEnd =  start.rotation * Quaternion.Euler(0.0f, 180.0f, 0.0f) *Quaternion.Inverse(end.rotation) * transform.rotation;
        StartCoroutine(correctRotation());
    }

    public void ApplyDamage(int damage)
    {
        hp = Mathf.Max(hp - (damage * resistanceMultiplier), 0.0f);
        
        _lastHit = Time.time;
        print($"{name} : currentHp {hp}");
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
        yield return new WaitForSecondsRealtime(0.2f);
        _rotationCorrectFlag = true;
    }
}
