using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class Sniper : Firearm
{
    [Header("Sniper")]
    // TODO: How to do sniper scopes? PLACEHOLDER TYPE
    [SerializeField] private Texture scopeTexture;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    protected override void Fire()
    {
        // TODO: Create Bullet(Vector3 origin, Quaternion direction, int speed, int maxRange): Needs collider, die on collision with any surface, die when past range
        
        --ammoCount;
        // TODO: Apply Recoil
        
        // TODO: NEED TO CHECK ???
        IsReady = false;
        // DONT DO COROUTINE
        StartCoroutine(StaggerShots());
    }
}
