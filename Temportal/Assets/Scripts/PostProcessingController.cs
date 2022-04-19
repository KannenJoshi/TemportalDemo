using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private float chromaticAberrationIntensity = 0.5f;
    [SerializeField] private float saturation = -50f;
    
    private ChromaticAberration chroma;
    private ColorAdjustments colourAdjust;
    
    private void Start()
    {
        postProcessingVolume.profile.TryGet(out chroma);
        postProcessingVolume.profile.TryGet(out colourAdjust);
    }

    private void Update()
    {
        if (PauseMenu.IsPaused) return;
        if (chroma == null || colourAdjust == null) return;

        var val = 0f;
        
        if (TimeManager.isBulletTime)
        {
            if (!chroma.IsActive())
            {
                chroma.intensity.value = 0f;
                chroma.active = true;
            }
            
            if (!colourAdjust.active)
            {
                colourAdjust.saturation.value = 0f;
                colourAdjust.active = true;
            }

            val = (1f/TimeManager.EnterTime) * Time.unscaledDeltaTime;
            
            if (!Mathf.Approximately(chroma.intensity.value, chromaticAberrationIntensity))
            {
                //var val = TimeManager.EnterTime * (Time.timeScale / TimeManager.BulletTimeScale);
                chroma.intensity.value = Mathf.Clamp(chroma.intensity.value + val, 0f, chromaticAberrationIntensity);
            }
            
            if (!Mathf.Approximately(colourAdjust.saturation.value, saturation))
            {
                //var val = TimeManager.EnterTime * (Time.timeScale / TimeManager.BulletTimeScale);
                val *= saturation;
                colourAdjust.saturation.value = Mathf.Clamp(colourAdjust.saturation.value + val, 0f, saturation);
            }
            
        }
        else if (!TimeManager.isBulletTime)
        {
            //var val = TimeManager.ExitTime * (Time.timeScale / TimeManager.BulletTimeScale);
            val = (1f/TimeManager.ExitTime) * Time.unscaledDeltaTime;
            
            if (!Mathf.Approximately(chroma.intensity.value, 0f))
            {
                chroma.intensity.value = Mathf.Clamp(chroma.intensity.value - val, 0f, chromaticAberrationIntensity);
            }
            else if (chroma.IsActive())
            {
                chroma.intensity.value = 0f;
                chroma.active = false;
            }
            
            if (!Mathf.Approximately(colourAdjust.saturation.value, 0f))
            {
                val *= saturation;
                colourAdjust.saturation.value = Mathf.Clamp(colourAdjust.saturation.value - val, 0f, saturation);
            }
            else if (colourAdjust.IsActive())
            {
                colourAdjust.saturation.value = 0f;
                colourAdjust.active = false;
            }
        }
    }
}