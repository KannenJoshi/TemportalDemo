using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PortalTraveller : MonoBehaviour
{
    [SerializeField] public Rigidbody rb;
    //[SerializeField] private Transform orientation;
    
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

    public virtual void Teleport(Transform start, Transform end)
    {
        //transform.position = pos;
        //transform.rotation = rot;
        
        //rb.velocity = end.TransformVector (start.InverseTransformVector (rb.velocity));
        //rb.angularVelocity = end.TransformVector (start.InverseTransformVector (rb.angularVelocity));
        
        transform.position = end.TransformPoint(Quaternion.Euler(0.0f, 180.0f, 0.0f) * start.InverseTransformPoint(transform.position));
        
        transform.rotation = end.rotation * (Quaternion.Euler(0.0f, 180.0f, 0.0f) * Quaternion.Inverse(start.rotation) * transform.rotation);
        //orientation.rotation = end.rotation * (Quaternion.Euler(0.0f, 180.0f, 0.0f) * Quaternion.Inverse(start.rotation) * orientation.rotation);
        
        rb.velocity = end.TransformVector(Quaternion.Euler(0.0f, 180.0f, 0.0f) * start.InverseTransformVector(rb.velocity));
        //rb.velocity = end.TransformVector(start.InverseTransformVector(rb.velocity));
        Physics.SyncTransforms();
    }
}
