using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

/*
 *  ALL LOGIC FOR CONTROLLING INTERACTIONS WITH PORTAL
 *  FOR RENDERING SEE PORTAL CAMERA
 */
[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [field: SerializeField] public Portal OtherPortal { get; private set; }
    [SerializeField] private Color outlineColour;
    
    private BoxCollider _wall;
    public Renderer Renderer { get; private set; }
    private MeshFilter ScreenMeshFilter { get; set; }

    private GameObject _clone;
    private List<PortalTraveller> _travellers = new List<PortalTraveller>();
    
    public bool IsPlaced { get; private set; }
    

    private void Awake()
    {
        _wall = GetComponent<BoxCollider>();
        Renderer = GetComponent<Renderer>();
        ScreenMeshFilter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        gameObject.SetActive(true);
        IsPlaced = true;
    }
    
    private void Update()
    {
        Renderer.enabled = OtherPortal.IsPlaced;
        
        // Tried foreach but got `InvalidOperationException: Collection was modified; enumeration operation may not execute.`
        //foreach (var traveller in _travellers)
        for (int i = 0; i < _travellers.Count; ++i)
        {
            var traveller = _travellers[i];
            
            // If traveller killed before exited remove
            if (traveller == null)
            {
                _travellers.RemoveAt(i);
                i--;
                continue;
            }

            Vector3 relativeObjPos = transform.InverseTransformPoint(traveller.transform.position);
            //var portalCameraPosition = OtherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * traveller.transform.localToWorldMatrix;

            if (relativeObjPos.z > 0.0f)
            {
                traveller.Teleport(transform, OtherPortal.transform);
                _travellers.RemoveAt(i);
                i--;
            }
        }
    }

    public Vector4 Render(int iterationID, Camera portalCam, ScriptableRenderContext SRC)
    {
        // check if need and iterID bit I added
        //if (iterationID != 0 && !CameraUtility.BoundsOverlap(ScreenMeshFilter, OtherPortal.ScreenMeshFilter, portalCam)) return Vector4.zero;

        // Get Position of this iteration by applying transform repeatedly
        for (var i = 0; i <= iterationID; ++i)
        {
            portalCam.transform.position = OtherPortal.transform.TransformPoint(Quaternion.Euler(0.0f, 180.0f, 0.0f) * transform.InverseTransformPoint(portalCam.transform.position));
            portalCam.transform.rotation = OtherPortal.transform.rotation * (Quaternion.Euler(0.0f, 180.0f, 0.0f) * (Quaternion.Inverse(transform.rotation) * portalCam.transform.rotation));
        }

        Plane p = new Plane(-OtherPortal.transform.forward, OtherPortal.transform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCam.worldToCameraMatrix)) * clipPlaneWorldSpace;
        return clipPlaneCameraSpace;
    }

    public void PlacePortal()
    {
        gameObject.SetActive(true);
        IsPlaced = true;
    }

    public void RemovePortal()
    {
        gameObject.SetActive(false);
        IsPlaced = false;
    }

    // Enter Hitbox
    private void OnTriggerEnter(Collider other)
    {
        print("PortalTrigger: " + other.tag);
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller != null)
        {
            _travellers.Add(traveller);
            traveller.EnterPortal();
        }
    }

    // Exit Hitbox
    private void OnTriggerExit(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller && _travellers.Contains(traveller))
        {
            _travellers.Remove(traveller);
            traveller.ExitPortal();
        }
    }
    
    // PreRender for Slices
    // Render Portal Manually
    // PostRender for Slices
    
    /*
     *  UTILITY
     */
    
    // https://github.com/SebLague/Portals/blob/53ff52abc836837eb248ffce980345fa645d817f/Assets/Scripts/Core/Portal.cs#L66
    public static bool BoundsOverlap (MeshFilter nearObject, MeshFilter farObject, Camera camera) {

        var near = CameraUtility.GetScreenRectFromBounds (nearObject, camera);
        var far = CameraUtility.GetScreenRectFromBounds (farObject, camera);

        // ensure far object is indeed further away than near object
        if (far.zMax > near.zMin) {
            // Doesn't overlap on x axis
            if (far.xMax < near.xMin || far.xMin > near.xMax) {
                return false;
            }
            // Doesn't overlap on y axis
            if (far.yMax < near.yMin || far.yMin > near.yMax) {
                return false;
            }
            // Overlaps
            return true;
        }
        return false;
    }
}
