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
    [SerializeField] private ParticleSystem explosionEffect;
    
    [Header("Advanced")]
    [SerializeField] private float selfDamageMultiplier = 0.2f;
    [Range(0.1f, 5f)]
    [SerializeField] private float damageAtEdgeScale = 2f;

    private bool exploded;
    
    private void OnDestroy()
    {
        // Create Explosion
        // https://www.youtube.com/watch?v=BYL6JtUdEY0
        var pos = transform.position;
        
        // Show Explosion
        if (explosionEffect != null && !exploded)
        {
            var eff = Instantiate(explosionEffect, pos, transform.rotation);
            //Destroy(eff, eff.main.duration);
            //eff.Simulate(0.25f);
            //eff.Play();
            
            //https://forum.unity.com/threads/how-to-scale-particle-system.476616/
            var main = eff.main;
            main.scalingMode = ParticleSystemScalingMode.Local;
            eff.transform.localScale = new Vector3(radius, radius, radius);
            
            exploded = true;
        }

        Collider[] colliders = Physics.OverlapSphere(pos, radius);
        foreach (Collider col in colliders)
        {
            var obj = col.gameObject;
            var objRb = obj.GetComponent<Rigidbody>();
            
            if (objRb != null)
            {
                var objEntity = obj.GetComponent<Entity>();
                var objAI = obj.GetComponent<NPC>();

                var dmg = 0.5f;
                
                if (objEntity != null)
                {
                    // If is one to shoot bullet reduce damage by multiplier
                    var multiplier = obj.CompareTag(_parentTag) ? selfDamageMultiplier : 1f;
                    
                    // Adjust according to https://www.desmos.com/calculator/snz1npz55j
                    var x = Vector3.Distance(obj.transform.position, pos);
                    var s = damageAtEdgeScale;
                    dmg = damage - ((damage + s * s) / (s * radius)) * x;
                    dmg *= multiplier;
                    dmg += s;
                    objEntity.ApplyDamage(Mathf.RoundToInt(dmg));
                }

                // ENABLE WHEN ENEMY HIT GROUND CHECK
                //if (objAI != null) objAI.enabled = false;
                
                //objRb.AddExplosionForce(explosiveForce, pos, radius, 0.5f, ForceMode.Impulse);
                objRb.AddExplosionForce(explosiveForce, pos, radius, dmg, ForceMode.Impulse);

            }
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // TODO: Edit ignore _parentTag to ignore at first shoot if _parentTag is same, not just for player so Enemy can use
        if (_ignoreShooterTag && collision.gameObject.tag.Equals(_parentTag)) return;
        
        if (collision.gameObject.tag.Equals("Player") || collision.gameObject.tag.Equals("Enemy"))
        {
            collision.gameObject.GetComponent<Entity>().ApplyDamage(Mathf.RoundToInt(damage/damageAtEdgeScale));
            Destroy(gameObject);
        }
        
        //print(collision.gameObject.tag);
        if (collision.gameObject.tag.Equals("Portal"))
        {
            //StartCoroutine(DisableTrailOnTeleport());
        }
    }
}
