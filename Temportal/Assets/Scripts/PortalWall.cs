using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PortalWall : MonoBehaviour
{
    [SerializeField] private Portal front;
    [SerializeField] private Portal back;
    private NavMeshObstacle _meshObstacle;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _meshObstacle = GetComponent<NavMeshObstacle>();
    }

    private void UpdateMesh()
    {
        if (front || back) _meshObstacle.enabled = false;
        else _meshObstacle.enabled = true;
    }

    public Collider Collider => _collider;

    public Portal Front
    {
        get => front;
        set
        {
            front = value;
            UpdateMesh();
        }
    }

    public Portal Back
    {
        get => back;
        set
        {
            back = value;
            UpdateMesh();
        }
    }
}
