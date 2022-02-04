using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPos;
    
    void Update()
    {
        //transform.position = cameraPos.position;
        transform.localEulerAngles = new Vector3(0, cameraPos.eulerAngles.y, 0);
        //transform.localEulerAngles = new Vector3(0, Mathf.Lerp(transform.localEulerAngles.y, cameraPos.eulerAngles.y, Time.smoothDeltaTime*10), 0);

        //transform.localRotation = Quaternion.Euler(new Vector3(0,
        //    Mathf.Lerp(transform.localEulerAngles.y, cameraPos.eulerAngles.y, 1), 0));
        
        //transform.localRotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y, 0));
    }
}
