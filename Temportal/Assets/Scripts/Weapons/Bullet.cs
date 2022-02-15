using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage = 0;
    private string fireTag;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move in bullet direction with speed
    }

    private void OnCollisionEnter(Collision collision)
    {
        throw new NotImplementedException();
    }

    public void SetStats(int damage, string tag)
    {
        
    }
}
