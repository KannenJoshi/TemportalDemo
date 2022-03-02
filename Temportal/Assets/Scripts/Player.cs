using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private Transform orientation;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected override void UpdateBehaviour()
    {

    }

    public override void Teleport(Transform start, Transform end)
    {
        transform.position =
            end.TransformPoint(Quaternion.Euler(0.0f, 180.0f, 0.0f) * start.InverseTransformPoint(transform.position));
        
        //transform.rotation = end.rotation * (Quaternion.Euler(0.0f, 180.0f, 0.0f) * Quaternion.Inverse(start.rotation) * transform.rotation);
        orientation.rotation = end.rotation * (Quaternion.Euler(0.0f, 180.0f, 0.0f) * Quaternion.Inverse(start.rotation) * orientation.rotation);

        rb.velocity =
            end.TransformVector(Quaternion.Euler(0.0f, 180.0f, 0.0f) * start.InverseTransformVector(rb.velocity));

        Physics.SyncTransforms();
    }
}
