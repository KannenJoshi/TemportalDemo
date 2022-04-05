using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PortalTraveller
{
    [SerializeField] private float damageMultiplier = 1.0f;
    [SerializeField] private Color playerColour;// = new Color(69, 210, 255);
    [SerializeField] private Color enemyColour;// = new Color(255, 103, 16);
    [Range(0, 5)] [SerializeField] private float emission;
    
    protected int damage = 10;
    protected string _parentTag;
    private TrailRenderer tr;
    private PortalTraveller pt;
    private MeshRenderer model;
    
    // So Doesn't hit entity shooting when created
    protected bool _ignoreShooterTag = true;

    protected override void Awake()
    {
        base.Awake();
        tr = GetComponent<TrailRenderer>();
        pt = GetComponent<PortalTraveller>();
        model = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
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
        if (collision.gameObject.tag.Equals("Portal"))
        {
            //StartCoroutine(DisableTrailOnTeleport());
        }
    }

    public override void EnterPortal()
    {
        tr.enabled = false;
    }

    public override void ExitPortal()
    {
        tr.enabled = true;
    }

    public void SetStats(int damage, string tag)
    {
        this.damage = Mathf.RoundToInt(damage * damageMultiplier);
        _parentTag = tag;

        //tr.material.color = _parentTag.Equals("Player") ? playerColour : enemyColour;
        var colour = tag.Equals("Player") ? playerColour : enemyColour;
        tr.material.SetColor("_Base", colour);
        var emissiveColour = colour * Mathf.LinearToGammaSpace(emission);
        model.material.EnableKeyword("_EMISSION");
        model.material.SetColor("_EmissionColor", emissiveColour);
        DynamicGI.SetEmissive(model, emissiveColour);
    }

    IEnumerator DelayFirstShotCollision()
    {
        yield return new WaitForSeconds(0.1f);
        _ignoreShooterTag = false;
    }

    /*IEnumerator DisableTrailOnTeleport()
    {
        tr.enabled = false;
        yield return new WaitForSeconds(0.1f);
        tr.enabled = true;
    }*/
}
