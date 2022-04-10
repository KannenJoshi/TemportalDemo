using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private bool isBulletTime;
    [SerializeField] private float bulletTimeScale = 0.05f;
    [SerializeField] private float enterTime = 0.5f;
    [SerializeField] private float exitTime = 2.0f;

    private float _fixedDT;

    private void Awake()
    {
        _fixedDT = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Don't change if game paused
        if (Time.timeScale == 0f) return;
        
        // Introduced temp var as if tried to directly subtract when at 0 throws error;
        var dTS = 0.0f;
        if (isBulletTime)
        {
            dTS -= (1f / enterTime) * Time.unscaledDeltaTime;
        }
        else
        {
            dTS += (1f / exitTime) * Time.unscaledDeltaTime;
        }

        Time.timeScale = Mathf.Clamp(Time.timeScale + dTS, bulletTimeScale, 1f);
        Time.fixedDeltaTime = Time.timeScale * _fixedDT;
    }

    public bool IsBulletTime
    {
        get => isBulletTime;
        set => isBulletTime = value;
    }

    public float BulletTimeScale => bulletTimeScale;
    public float EnterTime => enterTime;
    public float ExitTime => exitTime;
}
