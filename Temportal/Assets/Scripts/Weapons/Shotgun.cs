using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Firearm
{
    [Header("Shotgun")]
    [SerializeField] private int bulletsPerClick = 3; //
    [SerializeField] private float spread = 0.2f;
    [SerializeField] private bool slugShot = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }


    protected override void Fire()
    {
        if (slugShot)
        {
            // TODO: Create Bullet(Vector3 origin, Quaternion direction, int speed, int maxRange): Needs collider, die on collision with any surface, die when past range
        }
        else
        {
            for (int i = 0; i < bulletsPerClick; i++)
            {
                // TODO: Create Bullet(Vector3 origin, Quaternion direction, int speed, int maxRange): Needs collider, die on collision with any surface, die when past range
                var a = 1;
            }
        }
        
        --ammoCount;
        // TODO: Apply Recoil
        
        // TODO: NEED TO CHECK ???
        IsReady = false;
        // DONT DO COROUTINE
        StartCoroutine(StaggerShots());
    }
}
