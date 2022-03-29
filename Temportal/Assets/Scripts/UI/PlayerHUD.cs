using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    /*
     [Header("Bars")]
    [SerializeField] private float healthCurrent;
    [SerializeField] private float healthMax;
    [SerializeField] private float temporalMeterCurrent;
    [SerializeField] private int temporalMeterMax;
    */
    
    [Header("Numbers")]
    [SerializeField] private TextMeshProUGUI ammoCount;
    [SerializeField] private TextMeshProUGUI ammoMax;

    [Header("Indicators")]
    [SerializeField] private Image LPortal;
    [SerializeField] private Image RPortal;
    [SerializeField] private Image reloadCircle;
    
    private Portal leftPortal;
    private Portal rightPortal;

    private float reloadTime;

    private void Awake()
    {
        var portals = GameObject.FindGameObjectsWithTag("Portal");
        leftPortal = portals[0].GetComponent<Portal>();
        rightPortal = portals[1].GetComponent<Portal>();
    }

    private void Update()
    {
        var tempColour = LPortal.color;
        tempColour.a = leftPortal.IsPlaced ? 1f : 0.2f;
        LPortal.color = tempColour;
        
        tempColour = RPortal.color;
        tempColour.a = rightPortal.IsPlaced ? 1f : 0.2f;
        RPortal.color = tempColour;

        if (reloadCircle.enabled)
        {
            reloadCircle.fillAmount += Time.deltaTime / reloadTime;
            if (reloadCircle.fillAmount >= 1.0f) reloadCircle.enabled = false;
        }
    }

    public void SetAmmo(int count) { ammoCount.text = count.ToString(); }
    public void SetAmmoMax(int max) { ammoMax.text = max.ToString(); }

    public void ShowReload(float reloadTime)
    {
        reloadCircle.enabled = true;
        reloadCircle.fillAmount = 0.0f;
        this.reloadTime = reloadTime;
    }
}
