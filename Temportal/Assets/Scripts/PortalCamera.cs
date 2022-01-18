using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


// https://www.youtube.com/watch?v=PkGjYig8avo&list=PL3AC9MS002vcWA8Siui3kMBhy0_tAGGch&index=47&t=208s


public class PortalCamera : MonoBehaviour
{
    [SerializeField] private Portal[] portals = Portal[2];
    [SerializeField] private Camera portalCamera;

    [SerializeField] private int iterations = 7;

    private RenderTexture tempTexture1;
    private RenderTexture tempTexture2;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();

        tempTexture1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        tempTexture2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    void Start()
    {
        portals[0].Renderer.material.mainTexture = tempTexture1;
        portals[1].Renderer.material.mainTexture = tempTexture2;
    }

    // Update is called once per frame
    void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        // Do not render cameras if both are not placed
        if (!portals[0].IsPlaced || !portals[1].IsPlaced)
        {
            return;
        }

        // For both portals
        for (int p = 0; p <= 1; p++)
        {
            // If visible then render
            if (portals[p].Renderer.isVisible)
            {
                portalCamera.targetTexture = tempTexture1;
            }
        }

        if (portals[0].Renderer.isVisible)
        {
            portalCamera.targetTexture = tempTexture1;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[0], portals[1], i, SRC);
            }
        }

        if (portals[1].Renderer.isVisible)
        {
            portalCamera.targetTexture = tempTexture2;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[1], portals[0], i, SRC);
            }
        }
    }

    // inPortal being rendered into
    private void RenderCamera(Portal inPortal, Portal outPortal, int iterationID, ScriptableRenderContext SRC)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for (int i = 1; i <= iterationID; ++i)
        {
            // Position the camera behind the other portal
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }


        // Set the camera's oblique view frustum
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
    }
}
