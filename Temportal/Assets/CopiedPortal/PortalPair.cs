using UnityEngine;

public class PortalPair : MonoBehaviour
{
    public PortalOld[] Portals { private set; get; }

    private void Awake()
    {
        Portals = GetComponentsInChildren<PortalOld>();

        if (Portals.Length != 2)
            Debug.LogError("PortalPair children must contain exactly two Portal components in total.");
    }
}