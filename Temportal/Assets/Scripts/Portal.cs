using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private bool IsPlaced = false;

    public Renderer Renderer { get => renderer; set => renderer = value; }
    public bool IsPlaced { get => IsPlaced; set => IsPlaced = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
