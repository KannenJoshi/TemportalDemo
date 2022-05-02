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
    private static readonly int DisplayMask = Shader.PropertyToID("displayMask");
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

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

    void RenderPortal(ScriptableRenderContext SRC, int id, RenderTexture tempTex)
    {
        //if (CameraUtility.VisibleFromCamera(portals[id].Renderer, mainCamera))
        if (!portals[id].Renderer.isVisible) return;
        
        portalCamera.targetTexture = tempTex;
        portals[1-id].Renderer.material.SetInt(DisplayMask, 0);

        /*int limit = !CameraUtility.VisibleFromCamera(portals[1-id].Renderer, portalCamera)
                ? recursions - 1
                : 0;*/

        portals[id].Renderer.enabled = false;
        portals[id].Renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        portals[1-id].Renderer.material.SetTexture(MainTex, tempTex);
            
        for (var i = recursions - 1; i >= 0; --i)
        {
            portalCamera.transform.position = transform.position;
            portalCamera.transform.rotation = transform.rotation;
                
            var clipPlaneCameraSpace = portals[1-id].Render(i, portalCamera, SRC);
            // If no need to use recursion then skip this iteration;
            if (clipPlaneCameraSpace.Equals(Vector4.zero)) continue;
                
            var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
            portalCamera.projectionMatrix = newMatrix;
                
            if (i > 0 && !CameraUtility.BoundsOverlap (portals[id].ScreenMeshFilter, portals[1-id].ScreenMeshFilter, portalCamera)) continue;

            //if (i > 0 && !CameraUtility.BoundsOverlap (portals[id].ScreenMeshFilter, portals[1-id].ScreenMeshFilter, portalCamera)) continue;
            //if (i > 0 && !CameraUtility.VisibleFromCamera(portals[1-id].Renderer, portalCamera)) continue;
                
            // Render the camera to its render target.
            UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
                
            portals[1-id].Renderer.material.SetInt(DisplayMask, 1);

            //
                
        }
        portals[id].Renderer.enabled = true;
        portals[id].Renderer.shadowCastingMode = ShadowCastingMode.On;
    }

    // Update is called once per frame
    void RenderCamera(ScriptableRenderContext SRC, Camera camera)
    {
        // If both portals placed, and a portal visible from Player Cam, render them
        if (!portals[0].IsPlaced || !portals[1].IsPlaced)
        {
            portals[0].Renderer.material.SetInt(DisplayMask, 0);
            portals[1].Renderer.material.SetInt(DisplayMask, 0);
            return;
        }
        
        RenderPortal(SRC, 0, tempTexL);
        RenderPortal(SRC, 1, tempTexR);
    }
}
