using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PortalTraveller : MonoBehaviour
{
    [SerializeField] public Rigidbody rb;
    [SerializeField] private Transform orientation;
    
    void Awake()
    {
        //rb = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    // When Touch Portal
    public void EnterPortal()
    {
        // Instantiate Clone if not
        // Set active if is
    }

    // When Exit (Not on Teleport)
    public void ExitPortal()
    {
        // Deactivate clone
    }

    public virtual void Teleport(Transform inPortal, Transform outPortal, Vector3 pos, Quaternion rot)
    {
        //transform.position = pos;
        //transform.rotation = rot;
        
        //rb.velocity = outPortal.TransformVector (inPortal.InverseTransformVector (rb.velocity));
        //rb.angularVelocity = outPortal.TransformVector (inPortal.InverseTransformVector (rb.angularVelocity));
        
        transform.position = outPortal.TransformPoint(Quaternion.Euler(0.0f, 180.0f, 0.0f) * inPortal.InverseTransformPoint(transform.position));
        
        //transform.rotation = outPortal.rotation * (Quaternion.Euler(0.0f, 180.0f, 0.0f) * Quaternion.Inverse(inPortal.rotation) * transform.rotation);
        orientation.rotation = outPortal.rotation * (Quaternion.Euler(0.0f, 180.0f, 0.0f) * Quaternion.Inverse(inPortal.rotation) * orientation.rotation);
        
        rb.velocity = outPortal.TransformVector(Quaternion.Euler(0.0f, 180.0f, 0.0f) * inPortal.InverseTransformVector(rb.velocity));
        //rb.velocity = outPortal.TransformVector(inPortal.InverseTransformVector(rb.velocity));
        Physics.SyncTransforms();
    }
}
