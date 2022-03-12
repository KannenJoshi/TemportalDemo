using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 1.0f;
    [SerializeField] private Color playerColour;// = new Color(69, 210, 255);
    [SerializeField] private Color enemyColour;// = new Color(255, 103, 16);

    private int damage = 10;
    private string _parentTag;
    private TrailRenderer tr;
    private PortalTraveller pt;
    
    // So Doesn't hit entity shooting when created
    private bool _ignoreShooterTag = true;

    void Awake()
    {
        tr = GetComponent<TrailRenderer>();
        pt = GetComponent<PortalTraveller>();
    }

    void Start()
    {
        StartCoroutine(DelayFirstShotCollision());
    }

    void OnCollisionEnter(Collision collision)
    {
        // TODO: Edit ignore _parentTag to ignore at first shoot if _parentTag is same, not just for player so Enemy can use
        if (_ignoreShooterTag && collision.gameObject.tag.Equals(_parentTag)) return;
        
        if (collision.gameObject.tag.Equals("Player") || collision.gameObject.tag.Equals("Enemy"))
        {
            collision.gameObject.GetComponent<Entity>().ApplyDamage(damage);
            Destroy(gameObject);
        }
        
        //print(collision.gameObject.tag);
        if (collision.gameObject.tag.Equals("PortalWall"))
        {
            //print("Bullet hit " + collision.gameObject.tag);
            StartCoroutine(DisableTrailOnTeleport());
            //Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), collision.collider);
            //var portal = collision.gameObject.GetComponent<Portal>();
            //pt.Teleport(portal.transform, portal.OtherPortal.transform);
        }
        else
            Destroy(gameObject);

    }

    public void SetStats(int damage, string tag)
    {
        this.damage = Mathf.RoundToInt(damage * damageMultiplier);
        this._parentTag = tag;

        //tr.material.color = _parentTag.Equals("Player") ? playerColour : enemyColour;
        tr.material.SetColor("_Base", tag.Equals("Player") ? playerColour : enemyColour);
    }

    IEnumerator DelayFirstShotCollision()
    {
        yield return new WaitForSeconds(0.1f);
        _ignoreShooterTag = false;
    }

    IEnumerator DisableTrailOnTeleport()
    {
        tr.enabled = false;
        yield return new WaitForEndOfFrame();
        tr.enabled = true;
    }
}
