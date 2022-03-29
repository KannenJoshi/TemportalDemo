using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

[RequireComponent(typeof(Camera))]
public class PortalCamera : MonoBehaviour
{
    //[SerializeField]
    [SerializeField] private Portal[] portals = new Portal[2];
    [SerializeField] private Camera portalCamera;
    [SerializeField] private int recursions = 5;

    private RenderTexture tempTexL;
    private RenderTexture tempTexR;
    
    private Camera mainCamera;
    
    private void Awake()
    {
        mainCamera = GetComponent<Camera>();

        tempTexL = new RenderTexture(Screen.width, Screen.height, 0);
        tempTexR = new RenderTexture(Screen.width, Screen.height, 0);
    }
    
    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += RenderCamera;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= RenderCamera;
    }

    void Start()
    {
        portals[0].Renderer.material.mainTexture = tempTexL;
        portals[1].Renderer.material.mainTexture = tempTexR;

        //enabled = true;
    }

    // Update is called once per frame
    void RenderCamera(ScriptableRenderContext SRC, Camera camera)
    {
        // If both portals placed, and a portal visible from Player Cam, render them
        if (!portals[0].IsPlaced || !portals[1].IsPlaced)
        {
            portals[0].Renderer.material.SetInt("displayMask", 0);
            portals[1].Renderer.material.SetInt("displayMask", 0);
            return;
        }
        
        //if (CameraUtility.VisibleFromCamera(portals[0].Renderer, mainCamera))
        if (portals[0].Renderer.isVisible)
        {
            portalCamera.targetTexture = tempTexL;
            portals[1].Renderer.material.SetInt("displayMask", 0);
            
            for (var i = recursions - 1; i >= 0; --i)
            {
                //portals[0].Renderer.enabled = false; 
                portals[0].Renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                portals[1].Renderer.material.SetTexture("_MainTex", tempTexL);
                
                portalCamera.transform.position = transform.position;
                portalCamera.transform.rotation = transform.rotation;
                
                var clipPlaneCameraSpace = portals[1].Render(i, portalCamera, SRC);
                if (clipPlaneCameraSpace.Equals(Vector4.zero)) continue;
                
                var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
                portalCamera.projectionMatrix = newMatrix;

                // Render the camera to its render target.
                UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
                
                portals[1].Renderer.material.SetInt("displayMask", 1);

                //portals[0].Renderer.enabled = true;
                portals[0].Renderer.shadowCastingMode = ShadowCastingMode.On;
            }
        }

        //if (CameraUtility.VisibleFromCamera(portals[1].Renderer, mainCamera))
        if (portals[1].Renderer.isVisible)
        {
            portalCamera.targetTexture = tempTexR;
            portals[0].Renderer.material.SetInt("displayMask", 0);
            
            for (var i = recursions - 1; i >= 0; --i)
            {
                //portals[1].Renderer.enabled = false;
                portals[1].Renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                portals[0].Renderer.material.SetTexture("_MainTex", tempTexR);
                
                portalCamera.transform.position = transform.position;
                portalCamera.transform.rotation = transform.rotation;
                
                var clipPlaneCameraSpace = portals[0].Render(i, portalCamera, SRC);
                if (clipPlaneCameraSpace.Equals(Vector4.zero)) continue;
                
                var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
                portalCamera.projectionMatrix = newMatrix;

                // Render the camera to its render target.
                UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);

                portals[0].Renderer.material.SetInt("displayMask", 1);

                //portals[1].Renderer.enabled = true;
                portals[1].Renderer.shadowCastingMode = ShadowCastingMode.On;
            }
        }
    }
}
