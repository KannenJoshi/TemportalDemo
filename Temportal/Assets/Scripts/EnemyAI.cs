using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : Entity
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Detect Player
        Heal();
    }

    private void Fire()
    {
        
    }

    private void Warp()
    {
        // Disappear 4D Shift Shader
        gameObject.SetActive(false);
    }

    private void CheckWarpEnd()
    {
        
    }
    
    
}
