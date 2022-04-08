using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayer : MonoBehaviour
{
    [SerializeField] private Transform cameraPos;
    [SerializeField] private Transform head;
    [SerializeField] private Transform orientation;

    void Awake()
    {
    }
    
    void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, cameraPos.eulerAngles.y + orientation.localEulerAngles.y, 0);

        var localRotation = head.localRotation;
        localRotation =
            Quaternion.Euler(new Vector3(cameraPos.eulerAngles.x, head.rotation.y, 0 ));
        head.localRotation = localRotation;
        
    }
}
