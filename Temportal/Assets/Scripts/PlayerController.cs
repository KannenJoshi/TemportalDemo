using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : PortalTraveller
{
    [Header("Tracking Variables")]
    // Current Movement
    [SerializeField] private Vector3 moveDir;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isRunning;
    [SerializeField] private bool isCrouching;
    // Is on the ground (true) or jumping (false)
    [SerializeField] private bool isGrounded;

    [Header("Movement")]
    // Movement
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float runMultiplier = 1.25f;
    [SerializeField] private float crouchMultiplier = 0.75f;
    // Jump
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float airResistance = 0.2f;
    // Ground
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundRadius = 0.3f;

    [Header("Components")]
    //[SerializeField] private new Rigidbody rb;
    //private Rigidbody rb;

    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private Transform head;

    private float _speed;
    private Vector2 _inputMovement;

    // Start is called before the first frame update
    private void Start()
    {
        //rb = base.GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }

    // FixedUpdate is called one per physics frame
    private void FixedUpdate()
    {
        GroundedCheck();
        if (!isGrounded)
        {
            HeadCheck();
        }
        //isGrounded = Physics.CheckCapsule(bounds.center,new Vector3(bounds.center.x,bounds.min.y-0.1f,bounds.center.z),0.18f));

        UpdateMove();
    }
    
    // Update is called once per frame
    private void Update()
    {
        //UpdateLook();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputMovement = context.ReadValue<Vector2>().normalized;
        if (context.performed)
        {
            isWalking = true;
        }
        else if (context.canceled)
        {
            //_speed = 0;
            isWalking = false;
        }
    }

    private void UpdateMove()
    {
        // Get input movement in world space
        moveDir = new Vector3(_inputMovement.x, 0f, _inputMovement.y);
        moveDir = transform.TransformDirection(moveDir);
        
        // Speed of Player by applying the multipliers
        _speed = moveSpeed;
        _speed *= isCrouching ? crouchMultiplier : isRunning ? runMultiplier : 1;
        
        
        // Less Airborne movement 
        float airControl = isGrounded ? 1 : airResistance; // Change only when new input while in air
        
        // Accelerate the Player
        rb.AddForce(moveDir * _speed * airControl, ForceMode.Acceleration); //ForceMode.Acceleration
        
        // Max Speed
        rb.velocity = new Vector3(0, rb.velocity.y, 0) + Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z), _speed); // Should not be done via velocity

        /*if (Vector3.Magnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z)) > _speed)
        {
            rb.velocity = (new Vector3(0, rb.velocity.y, 0) + Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z), _speed));
        }*/
        
        // Brake force - https://answers.unity.com/questions/9985/limiting-rigidbody-velocity.html
        /*float currentSpeed = rb.velocity.magnitude;
        if (currentSpeed > _speed)
        {
            float brakeSpeed = _speed - currentSpeed;
            Vector3 brakeVelocity = rb.velocity.normalized * brakeSpeed;
            rb.AddRelativeForce(brakeVelocity, ForceMode.Acceleration);
        }*/
    }


    /*public void OnLook(InputAction.CallbackContext context)
    {
        lookDelta = context.ReadValue<Vector2>() / Time.deltaTime;
    }*/

    /*private void UpdateLook()
    {
        var leftRightAngle = lookDelta.x * lookSpeed * Time.deltaTime;
        upDownAngle += lookDelta.y * -lookSpeed * Time.deltaTime;
        upDownAngle = Mathf.Clamp(upDownAngle, lookClamp.x, lookClamp.y);

        var localRotation = head.transform.localRotation;
        localRotation =
            Quaternion.Euler(new Vector3(upDownAngle, localRotation.eulerAngles.y, localRotation.eulerAngles.z));
        head.transform.localRotation = localRotation;
        transform.Rotate(new Vector3(0, leftRightAngle, 0));
    }*/
    
    
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed) 
            isRunning = true;
        else if (context.canceled) 
            isRunning = false;
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed) 
            isCrouching = true;
        else if (context.canceled) 
            isCrouching = false;
    }
    
    public void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed && isGrounded)
        {   
            rb.AddForce(new Vector3(0f, jumpForce * rb.mass, 0f), ForceMode.Impulse);
            //isGrounded = false;
        }
    }
    
    private void GroundedCheck()
    {
        var offSetHeight = new Vector3(0, capsuleCollider.center.y - capsuleCollider.height / 2f, 0);
        var hitGround = Physics.CheckSphere(transform.position + offSetHeight, groundRadius,
            groundMask, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(capsuleCollider.center, new Vector3(0, capsuleCollider.center.y - capsuleCollider.height / 2f, 0));
        isGrounded = hitGround;
    }
    private void HeadCheck()
    {
        var offSetHeight = new Vector3(0, capsuleCollider.center.y + capsuleCollider.height / 2f, 0);
        RaycastHit hit;
        
        // var hitHead = Physics.Raycast(transform.position + offSetHeight, groundRadius,
        //     groundMask, QueryTriggerInteraction.Ignore);
        //if (hitHead)
        if (Physics.Raycast(transform.position + offSetHeight, Vector3.up, out hit, groundRadius, groundMask))
        {
            //jumpForce = Mathf.Clamp(jumpForce, jumpForce, 0);
            rb.velocity.Set(rb.velocity.x, 0, rb.velocity.y);
        }
    }
}