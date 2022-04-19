using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

public class Burst : Firearm
{
    [Header("Burst")]
    [SerializeField] private int bulletsInSuccession = 3;
    [SerializeField] private float fireDelay = 0.8f;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
