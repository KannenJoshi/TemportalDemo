using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PortalTraveller : MonoBehaviour
{
    [SerializeField] public Rigidbody rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void EnterPortal()
    {
        
    }

    public void ExitPortal()
    {
        
    }

    public virtual void Teleport(Transform inPortal, Transform outPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
        
        rb.velocity = outPortal.TransformVector (inPortal.InverseTransformVector (rb.velocity));
        rb.angularVelocity = outPortal.TransformVector (inPortal.InverseTransformVector (rb.angularVelocity));
    }
}
