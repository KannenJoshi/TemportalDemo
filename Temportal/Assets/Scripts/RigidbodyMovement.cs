using UnityEngine;

public class RigidbodyMovement : MonoBehaviour
{
    // https://www.youtube.com/watch?v=b1uoLBp2I1w

    [Header("Player Components")] 
    [SerializeField] private Transform Camera;
    [SerializeField] private Rigidbody Body;
    [Space(5)]
    [Header("Player Variables")]
    [SerializeField] private float speed = 6;
    [SerializeField] private float sensitivity = 1; // Mouse sens
    [SerializeField] private float jumpForce = 15;

    private Vector3 playerMovementInput;
    private Vector3 playerMouseInput;


    void Update()
    {
        
    }

    public void Move()
    {
        
    }

    public void Look()
    {

    }

    public void Jump()
    {

    }
}
