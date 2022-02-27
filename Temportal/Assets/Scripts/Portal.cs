using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  ALL LOGIC FOR CONTROLLING INTERACTIONS WITH PORTAL
 *  FOR RENDERING SEE PORTAL CAMERA
 */
[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [field: SerializeField] public Portal OtherPortal { get; private set; }
    [SerializeField] private int recursions = 5;
    [SerializeField] private Color outlineColour;
    
    private BoxCollider _wall;
    private Renderer _renderer;
    private GameObject _clone;
    private List<PortalTraveller> _travellers = new List<PortalTraveller>();
    
    public bool IsPlaced { get; private set; }
    

    private void Awake()
    {
        _wall = GetComponent<BoxCollider>();
        _renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        //gameObject.SetActive(false);
    }
    
    private void LateUpdate()
    {
        //_renderer.enabled = OtherPortal.IsPlaced;
        
        // Tried foreach but got `InvalidOperationException: Collection was modified; enumeration operation may not execute.`
        //foreach (var traveller in _travellers)
        for (int i = 0; i < _travellers.Count; ++i)
        {
            var traveller = _travellers[i];
            
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

    public void PlacePortal()
    {
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
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller != null)
        {
            _travellers.Add(traveller);
            //traveller.enterPortal
        }
    }

    // Exit Hitbox
    private void OnTriggerExit(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller && _travellers.Contains(traveller))
        {
            _travellers.Remove(traveller);
            //traveller.exitPortal
        }
    }
    
    // PreRender for Slices
    // Render Portal Manually
    // PostRender for Slices
    
    /*
     *  UTILITY
     */
    
    // https://github.com/SebLague/Portals/blob/53ff52abc836837eb248ffce980345fa645d817f/Assets/Scripts/Core/Portal.cs#L66
    public static bool VisibleFromCamera (Renderer renderer, Camera camera) {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }
}
