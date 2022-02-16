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
    private string tag;
    private TrailRenderer tr;

    private bool ignorePlayerTag = true;

    void Awake()
    {
        tr = GetComponent<TrailRenderer>();
    }

    void Start()
    {
        StartCoroutine(DelayPlayer());
    }

    void OnCollisionEnter(Collision collision)
    {
        // TODO: Edit ignore tag to ignore at first shoot if tag is same, not just for player so Enemy can use
        if ((collision.gameObject.tag.Equals("Player") && !ignorePlayerTag) || collision.gameObject.tag.Equals("Enemy"))
        {
            collision.gameObject.GetComponent<Entity>().ApplyDamage(damage);
            Destroy(gameObject);
        }
    }

    public void SetStats(int damage, string tag)
    {
        this.damage = Mathf.RoundToInt(damage * damageMultiplier);
        this.tag = tag;

        tr.material.color = tag.Equals("Player") ? playerColour : enemyColour;
    }

    IEnumerator DelayPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        ignorePlayerTag = false;
    }
}
