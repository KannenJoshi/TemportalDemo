using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private float chromaticAberrationIntensity = 0.5f;
    [SerializeField] private float colourSaturation = 50f;
    [SerializeField] private float vignetteIntensity = 0.45f;
    [SerializeField] private float vignetteIntensityMaxIncrement = 0.5f;
    [SerializeField] private Player player;
    
    private ChromaticAberration chroma;
    private ColorAdjustments colourAdjust;
    private Vignette vignette;
    private Color defaultVignetteColour;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
        postProcessingVolume.profile.TryGet(out chroma);
        postProcessingVolume.profile.TryGet(out colourAdjust);
        postProcessingVolume.profile.TryGet(out vignette);

        if (vignette) defaultVignetteColour = vignette.color.value;
    }

    private void Update()
    {
        if (PauseMenu.IsPaused) return;

        var bt = TimeManager.isBulletTime;
        var windUp = bt ? TimeManager.EnterTime : TimeManager.ExitTime;
        var val = (1f/windUp) * Time.unscaledDeltaTime;
        
        // Bullet Time
        ChromaticAberration(bt, val);
        ColourAdjustment(bt, val*colourSaturation);
        
        // Player
        Vignette(player.Hp, player.HpMax);
    }
    
    private void ChromaticAberration(bool bt, float val)
    {
        if (chroma == null) return;
        
        if (bt)
        {
            if (!chroma.IsActive())
            {
                chroma.intensity.value = 0f;
                chroma.active = true;
            }
            
            if (!Mathf.Approximately(chroma.intensity.value, chromaticAberrationIntensity))
            {
                chroma.intensity.value = Mathf.Clamp(chroma.intensity.value + val, 0f, chromaticAberrationIntensity);
            }
        }
        else
        {
            if (!Mathf.Approximately(chroma.intensity.value, 0f))
            {
                chroma.intensity.value = Mathf.Clamp(chroma.intensity.value - val, 0f, chromaticAberrationIntensity);
            }
            else if (chroma.IsActive())
            {
                chroma.intensity.value = 0f;
                chroma.active = false;
            }
        }
    }
    
    private void ColourAdjustment(bool bt, float val)
    {
        if (colourAdjust == null) return;
        
        if (bt)
        {
            if (!colourAdjust.active)
            {
                colourAdjust.saturation.value = 0f;
                //colourAdjust.active = true;
            }
            
            if (!Mathf.Approximately(colourAdjust.saturation.value, colourSaturation))
            {
                //var val = TimeManager.EnterTime * (Time.timeScale / TimeManager.BulletTimeScale);
                colourAdjust.saturation.value = Mathf.Clamp(colourAdjust.saturation.value + val, 0f, colourSaturation);
            }
        }
        else
        {
            if (!Mathf.Approximately(colourAdjust.saturation.value, 0f))
            {
                val *= colourSaturation;
                colourAdjust.saturation.value = Mathf.Clamp(colourAdjust.saturation.value - val, 0f, colourSaturation);
            }
            else if (colourAdjust.IsActive())
            {
                colourAdjust.saturation.value = 0f;
                //colourAdjust.active = false;
            }
        }
    }
    
    private void Vignette(float hp, float hpMax)
    {
        if (vignette == null) return;

        var hpHalf = hpMax / 2f;
        var val = 1 - (hp / hpHalf);
        //var current = vignette.intensity.value;
        //var goal = vignetteIntensity + vignetteIntensityMaxIncrement * val;

        if (hp < hpHalf)
        {
            vignette.color.value = Color.red * val;
            vignette.intensity.value = vignetteIntensity + vignetteIntensityMaxIncrement * val;;
            
            // Lerp Intensity and Colour
            /*
            // Lerp Direction and Time
            val *= 5f * Mathf.Sign(goal - current);
            val *= Time.unscaledDeltaTime;
                 
            if (!Mathf.Approximately(current, goal))
            {
                vignette.intensity.value += val;
            }
            
            if (!Mathf.Approximately(
            */
        }
        else if (!vignette.color.value.Equals(defaultVignetteColour))
        {
            vignette.color.value = defaultVignetteColour;
            vignette.intensity.value = vignetteIntensity;
        }
    }
}