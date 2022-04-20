using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Drone : NPC
{
    [Header("Eye Colour")]
    [SerializeField] private Renderer eye;
    [SerializeField] private float emissionIntensity = 3.9f;
    [SerializeField] private Color defaultEyeColour;
    [SerializeField] private Color[] eyeColoursForStates;
    private bool fetchedEyeColour;
    
    [Header("Light Colour")]
    [SerializeField] private Light spotlight;
    [SerializeField] private Color defaultLightColour;
    [SerializeField] private Color[] lightColoursForStates;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    protected override void Awake()
    {
        base.Awake();
        
        target = GameObject.FindGameObjectWithTag("Player");
        
        var c = new Color();
        
        if (defaultEyeColour.Equals(c))
        {
            defaultEyeColour = eye.material.GetColor(EmissionColor);
            fetchedEyeColour = true;
        }
        if (defaultLightColour.Equals(c))
        {
            defaultLightColour = spotlight.color;
        }
        
        
        for (var i = 0; i < Enum.GetNames(typeof(AIState)).Length; i++)
        {
            if (eyeColoursForStates[i].Equals(c))
            {
                eyeColoursForStates[i] = defaultEyeColour;
            }

            if (lightColoursForStates[i].Equals(c))
                lightColoursForStates[i] = defaultLightColour;
        }
        
        SetColours((int)initialState);
        
        
    }

    private void Start()
    {
        
    }

    private void SetColours(int index)
    {
        var i = !fetchedEyeColour ? emissionIntensity : 1;
        var col = eyeColoursForStates[index] * i / eyeColoursForStates[index].grayscale;
        eye.material.SetColor(EmissionColor, col);
        spotlight.color = lightColoursForStates[index];
    }
    
    protected override void Idle()
    {
        base.Idle();
        SetColours(0);
    }

    protected override void Patrol()
    {
        base.Patrol();
        SetColours(1);
    }

    protected override void Chase()
    {
        base.Chase();
        SetColours(2);
    }

    protected override void Attack()
    {
        base.Attack();
        SetColours(3);
    }
}
