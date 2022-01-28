using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Tracking Variables")]
    // Current Movement
    [SerializeField] private Vector3 moveDir;
    [SerializeField] private float speeds;
    // Change in look angle
    [SerializeField] private Vector2 lookDelta;
    // Current camera angle (up/down)
    [SerializeField] private float upDownAngle;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isRunning;
    [SerializeField] private bool isCrouching;
    // Is on the ground (true) or jumping (false)
    [SerializeField] private bool isGrounded;

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float runMultiplier = 1.25f;
    [SerializeField] private float crouchMultiplier = 0.75f;
    [SerializeField] private float lookSpeed = 1f;
    // Max angle look up/down
    [SerializeField] private Vector2 lookClamp = new Vector2(-85, 85);
    // Jump Force
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float groundRadius = 0.3f;
    // Ground Layer
    [SerializeField] private LayerMask groundMask;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform head;
    //[SerializeField] private Transform orientation;

    private float _speed;
    private Vector2 _inputMovement;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }

    // FixedUpdate is called one per physics frame
    private void FixedUpdate()
    {
        //isGrounded = Physics.CheckCapsule(bounds.center,new Vector3(bounds.center.x,bounds.min.y-0.1f,bounds.center.z),0.18f));

        UpdateMove();
        // if ((characterController.collisionFlags & CollisionFlags.Above) != 0)
        // {
        //     HeadCheck();
        // }
    }
    
    // Update is called once per frame
    private void Update()
    {
        UpdateLook();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _inputMovement = context.ReadValue<Vector2>().normalized;
            isWalking = true;
        }
        else if (context.canceled)
        {
            //_speed = 0;
            _inputMovement = Vector2.zero;
            isWalking = false;
        }
    }

    private void UpdateMove()
    {
        moveDir = new Vector3(_inputMovement.x, 0f, _inputMovement.y);
        
        moveDir = transform.TransformDirection(moveDir);
        //var transform1 = transform;
        //moveDir = Quaternion.AngleAxis(transform1.eulerAngles.y, transform1.up) * moveDir;
        
        _speed = moveSpeed;
        _speed *= isCrouching ? crouchMultiplier : isRunning ? runMultiplier : 1;
        rb.AddForce(moveDir * 10, ForceMode.Acceleration); //ForceMode.Acceleration
        rb.velocity = rb.velocity.magnitude > 0.001
            ? Vector3.ClampMagnitude(rb.velocity, _speed)
            : Vector3.zero;
    }


    public void OnLook(InputAction.CallbackContext context)
    {
        lookDelta = context.ReadValue<Vector2>() / Time.deltaTime;
    }

    private void UpdateLook()
    {
        var leftRightAngle = lookDelta.x * lookSpeed * Time.deltaTime;
        upDownAngle += lookDelta.y * -lookSpeed * Time.deltaTime;
        upDownAngle = Mathf.Clamp(upDownAngle, lookClamp.x, lookClamp.y);

        var localRotation = head.localRotation;
        localRotation =
            Quaternion.Euler(new Vector3(upDownAngle, localRotation.eulerAngles.y, localRotation.eulerAngles.z));
        head.localRotation = localRotation;
        transform.Rotate(new Vector3(0, leftRightAngle, 0));
    }
    
    
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed) isRunning = true;
        else if (context.canceled) isRunning = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed) isCrouching = true;
        else if (context.canceled) isCrouching = false;
    }
    
    // public void OnJump(InputAction.CallbackContext callbackContext)
    // {
    //     if (callbackContext.performed)
    //         rb.AddForce(new Vector3(0f, jumpForce * rb.mass, 0f), ForceMode.Impulse);
    // }
    
    // public void HeadCheck()
    // {
    //     var offSetHeight = new Vector3(0, characterController.center.y + characterController.height / 2f, 0);
    //     var hitHead = Physics.CheckSphere(characterController.transform.position + offSetHeight, groundRadius,
    //         groundMask, QueryTriggerInteraction.Ignore);
    //     if (hitHead) jumpVelocity = Mathf.Clamp(jumpVelocity, jumpVelocity, 0);
    // }
}