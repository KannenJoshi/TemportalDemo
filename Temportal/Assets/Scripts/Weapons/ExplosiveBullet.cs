using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ExplosiveBullet : Bullet
{
    [Header("Explosive")]
    [SerializeField] private float radius = 10.0f;
    [SerializeField] private float explosiveForce = 700.0f;
    [SerializeField] private GameObject explosionEffect;
    
    [Header("Advanced")]
    [SerializeField] private float selfDamageMultiplier = 0.2f;
    [SerializeField] private float damageAtEdgeScale = 2f;

    private void OnDestroy()
    {
        // Create Explosion
        // https://www.youtube.com/watch?v=BYL6JtUdEY0
        var pos = transform.position;
        
        // Show Explosion
        //Instantiate(explosionEffect, pos, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(pos, radius);
        foreach (Collider col in colliders)
        {
            var obj = col.gameObject;
            var objRb = obj.GetComponent<Rigidbody>();
            
            if (objRb != null)
            {
                var objEntity = obj.GetComponent<Entity>();
                var objAI = obj.GetComponent<NPC>();
                
                if (objEntity != null)
                {
                    // If is one to shoot bullet reduce damage by multiplier
                    var multiplier = obj.CompareTag(_parentTag) ? selfDamageMultiplier : 1f;
                    
                    // Adjust according to https://www.desmos.com/calculator/snz1npz55j
                    var x = Vector3.Distance(obj.transform.position, pos);
                    var s = damageAtEdgeScale;
                    var dmg = damage - ((damage + s * s) / (s * radius)) * x;
                    dmg *= multiplier;
                    dmg += s;
                    objEntity.ApplyDamage(Mathf.RoundToInt(dmg));
                }

                // ENABLE WHEN ENEMY HIT GROUND CHECK
                //if (objAI != null) objAI.enabled = false;
                
                objRb.AddExplosionForce(explosiveForce, pos, radius, 0.5f, ForceMode.Impulse);

            }
        }
    }
}
