using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private int hpMax = 100;
    [SerializeField] private float hp = 100.0f;
    [SerializeField] private bool regenerate = true;
    [SerializeField] private float healAfterDamageDelay = 3.0f;
    [SerializeField] private float healTime = 0.1f; //(hp)s-1
    [SerializeField] private int healAmount = 5;
    
    private float _lastHit;
    private float _lastHeal;
    //private float _healTime;
    void Awake()
    {
        hp = hpMax;
        _lastHit = Time.time;
        _lastHeal = Time.time;
        //_healTime = 1 / healRate;
    }

    // Update is called once per frame
    void Update()
    {
        Heal();
        // UpdateBehaviour(); This gets overriden in subclasses to make do stuff ???
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

    public void ApplyDamage(int damage)
    {
        hp = Mathf.Max(hp - damage, 0.0f);
        
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
}
