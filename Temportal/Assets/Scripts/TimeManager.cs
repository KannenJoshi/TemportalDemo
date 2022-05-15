using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static bool isBulletTime;
    
    [SerializeField] private static float bulletTimeScale = 0.05f;
    [SerializeField] private static float enterTime = 0.5f;
    [SerializeField] private static float exitTime = 2.0f;

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
            if (Time.fixedDeltaTime > bulletTimeScale * _fixedDT)
            {
                Time.fixedDeltaTime = bulletTimeScale * _fixedDT;
            }
            dTS -= (1f / enterTime) * Time.unscaledDeltaTime;
        }
        else
        {
            if (Time.fixedDeltaTime < _fixedDT)
            {
                Time.fixedDeltaTime = _fixedDT;
            }
            dTS += (1f / exitTime) * Time.unscaledDeltaTime;
        }

        Time.timeScale = Mathf.Clamp(Time.timeScale + dTS, bulletTimeScale, 1f);
        //Time.fixedDeltaTime = Time.timeScale * _fixedDT;
    }
    
    public static void SetBulletTime(bool on)
    {
        isBulletTime = on;
    }

    public static float BulletTimeScale => bulletTimeScale;
    public static float EnterTime => enterTime;
    public static float ExitTime => exitTime;
}
