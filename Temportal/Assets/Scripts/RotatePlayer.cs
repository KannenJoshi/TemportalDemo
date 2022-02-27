using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
        // Rotate Model
        transform.localEulerAngles = new Vector3(0, cameraPos.eulerAngles.y + orientation.localEulerAngles.y, 0);
        //transform.localEulerAngles = new Vector3(0, orientation.localEulerAngles.y, 0);

        // Rotate Other
        //head.transform.localEulerAngles = new Vector3(cameraPos.eulerAngles.x, cameraPos.eulerAngles.y, 0);
        
        var localRotation = head.localRotation;
        localRotation =
            Quaternion.Euler(new Vector3(cameraPos.eulerAngles.x, head.rotation.y, 0 ));
        head.localRotation = localRotation;
        
        //transform.localEulerAngles = new Vector3(0, cameraPos.eulerAngles.y + transform.localEulerAngles.y, 0);


        //head.transform.Rotate(new Vector3(cameraPos.rotation.x, cameraPos.rotation.y, 0), Space.Self);
        // head.transform.RotateAround(transform.position, new Vector3(cameraPos.rotation.x, 0, 0), cameraPos.rotation.x);
        // head.transform.RotateAround(transform.position, new Vector3(0, cameraPos.rotation.y, 0), cameraPos.rotation.y);

        // var localRotation = head.transform.localRotation;
        // localRotation =
        //     Quaternion.Euler(new Vector3(cameraPos.localRotation.x, localRotation.eulerAngles.y, localRotation.eulerAngles.z));
        // head.transform.localRotation = localRotation;
    }
}
