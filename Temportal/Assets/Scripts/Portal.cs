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
[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    [field: SerializeField] public Color Colour { get; private set; }
    [field: SerializeField] public Portal OtherPortal { get; private set; }
    [field: SerializeField] public PortalWall Wall { get; set; }
    public Renderer Renderer { get; private set; }
    private MeshFilter ScreenMeshFilter { get; set; }
    private GameObject _clone;
    private List<PortalTraveller> _travellers = new List<PortalTraveller>();

    public bool IsPlaced { get; private set; }


    private void Awake()
    {
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
        Renderer.enabled = IsPlaced;
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

            Vector3 relativeObjPos = transform.InverseTransformPoint(traveller.TeleportThresholdTransform.position);
            //var portalCameraPosition = OtherPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * traveller.transform.localToWorldMatrix;

            if (relativeObjPos.z > 0.0f)
            {
                traveller.Teleport(transform, OtherPortal.transform);
                _travellers.RemoveAt(i);
                i--;
            }
        }
        Debug.DrawLine(this.transform.position, this.transform.position + this.transform.forward);

    }

    public Vector4 Render(int iterationID, Camera portalCam, ScriptableRenderContext SRC)
    {
        // check if need and iterID bit I added
        //if (iterationID != 0 && !CameraUtility.BoundsOverlap(ScreenMeshFilter, OtherPortal.ScreenMeshFilter, portalCam)) return Vector4.zero;

        // Get Position of this iteration by applying transform repeatedly
        for (var i = 0; i <= iterationID; ++i)
        {
            portalCam.transform.position = OtherPortal.transform.TransformPoint(Quaternion.Euler(0.0f, 180.0f, 0.0f) *
                transform.InverseTransformPoint(portalCam.transform.position));
            portalCam.transform.rotation = OtherPortal.transform.rotation * (Quaternion.Euler(0.0f, 180.0f, 0.0f) *
                                                                             (Quaternion.Inverse(transform.rotation) *
                                                                              portalCam.transform.rotation));
        }

        Plane p = new Plane(-OtherPortal.transform.forward, OtherPortal.transform.position);
        Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCam.worldToCameraMatrix)) * clipPlaneWorldSpace;
        return clipPlaneCameraSpace;
    }

    public void PlacePortal()
    {
        //Wall = GetComponentInParent<Collider>();
        gameObject.SetActive(true);
        //Renderer.material.SetColor("_MainTex", Colour);
        IsPlaced = true;
        
    }

    public void RemovePortal()
    {
        if (Wall)
        {
            if (Equals(Wall.Front)) Wall.Front = null;
            else Wall.Back = null;
        }
        Wall = null;
        gameObject.SetActive(false);
        IsPlaced = false;
        
        // Reset Rotation
        transform.rotation = Quaternion.identity;
    }

    // Enter Hitbox
    private void OnTriggerEnter(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller != null && OtherPortal.IsPlaced)
        {
            Physics.IgnoreCollision(other, Wall.Collider, true);
            traveller.EnterPortal();
            _travellers.Add(traveller);
        }
    }

    // Exit Hitbox
    private void OnTriggerExit(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller && _travellers.Contains(traveller))
        {
            Physics.IgnoreCollision(other, Wall.Collider, false);
            traveller.ExitPortal();
            _travellers.Remove(traveller);
        }
    }

    // PreRender for Slices
    // Render Portal Manually
    // PostRender for Slices
    
}