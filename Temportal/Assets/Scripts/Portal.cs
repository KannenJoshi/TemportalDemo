using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [SerializeField] public Portal OtherPortal;

    private RenderTexture texture;

    private BoxCollider Collider;
    private Renderer Renderer;
    
    public bool isPlaced;

    private void Awake()
    {
        Collider = GetComponent<BoxCollider>();
        Renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        Renderer.enabled = OtherPortal.isPlaced;
    }

    public void PlacePortal()
    {
        
    }

    public void RemovePortal()
    {
        gameObject.SetActive(false);
        isPlaced = false;
    }
}
