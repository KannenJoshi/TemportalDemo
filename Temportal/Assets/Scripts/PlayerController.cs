using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
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

    [Header("Look")]
    [SerializeField] private float lookSpeed = 1f;
    [SerializeField] private Vector2 lookClamp = new Vector2(-70,70);

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider capsuleCollider;
    
    private Portal leftPortal;
    private Portal rightPortal;

    private float _speed;
    private Vector2 _inputMovement;
    private Vector2 lookDelta;
    private float upDownAngle;

    private Transform head;
    private Transform hand;
    private Firearm weapon;

    private void Awake()
    {
        var portals = GameObject.FindGameObjectsWithTag("Portal");
        leftPortal = portals[0].GetComponent<Portal>();
        rightPortal = portals[1].GetComponent<Portal>();
    }
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        var portals = GameObject.FindGameObjectsWithTag("Portal");
        leftPortal = portals[0].GetComponent<Portal>();
        rightPortal = portals[1].GetComponent<Portal>();

        head = transform.GetChild(1);
        hand = head.GetChild(0);
        weapon = hand.GetChild(0).GetComponent<Firearm>();
        weapon.IsReady = true;
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
        UpdateLook();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputMovement = context.ReadValue<Vector2>().normalized;
        // Less Airborne movement input
        //_inputMovement *= isGrounded ? 1 : airResistance;
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
        moveDir *= isGrounded ? 1 : airResistance; // Change only when new input while in air
        
        // Accelerate the Player
        rb.AddForce(moveDir * _speed, ForceMode.Acceleration); //ForceMode.Acceleration
        
        // Max Speed
        rb.velocity = new Vector3(0, rb.velocity.y, 0) + Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z), _speed); // Should not be done via velocity
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

         var localRotation = head.transform.localRotation;
         localRotation =
             Quaternion.Euler(new Vector3(upDownAngle, localRotation.eulerAngles.y, localRotation.eulerAngles.z));
         head.transform.localRotation = localRotation;
         transform.Rotate(new Vector3(0, leftRightAngle, 0));
     }
    
    
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

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && weapon.IsReady && !weapon.IsReloading && !weapon.IsShooting)
        {
            weapon.IsShooting = true;
        }

        if (context.canceled)
        {
            weapon.IsShooting = false;
        }
    }
    
    public void OnSecondaryFire(InputAction.CallbackContext context)
    {
        
    }
    
    public void OnReload(InputAction.CallbackContext context)
    {
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        
    }

    private void SetPortalLocationAndDirection(Portal portal)
    {
        if (!Physics.Raycast(head.position, head.forward, out var hit)) return;
        if (!hit.transform.CompareTag("PortalWall")) return;
        
        // CHECK INSTEAD IF PORTAL ALREADY IN POSITION
        if (!hit.collider.Equals(leftPortal.Wall) && !hit.collider.Equals(rightPortal.Wall))
        {
            // POSITION THE NEW
            var portalWall = hit.collider.gameObject;
            var portalWallFwd = portalWall.transform.forward;
            var rotation = portalWall.transform.rotation;
            var portalSide = PortalSideCheck(hit.normal, portalWallFwd);

            // PORTAL DIRECTION
            if (portalSide == 0.0f) return;
            
            // REMOVE THE OLD
            portal.RemovePortal();
            
            var forwardsCheck = Vector3.Dot(portalWallFwd, rotation * portal.transform.forward);
            //forwardsCheck /= Mathf.Abs(forwardsCheck); // To get Sign Value
            
            print("portalSide");
            print(Mathf.Sign(portalSide));
            print("forwardsCheck");
            print(Mathf.Sign(forwardsCheck));
            
            // If hit front and portal fwd in same direction, or hits back and PFwd opposite
            if (Mathf.Sign(portalSide).Equals(Mathf.Sign(forwardsCheck)))
            {
                // Rotate so forward faces into wall
                print("rotate");
                portal.transform.Rotate(Quaternion.Inverse(rotation) * new Vector3(0, 180, 0));
            }
            portal.transform.Rotate(rotation.eulerAngles);
            
                        
            // PORTAL LOCATION
            portal.transform.position = portalWall.transform.position + (rotation * new Vector3(1.5f, 1.5f, portalSide*portalWall.transform.localScale.z));
            
            // PORTAL WALL COLLIDER
            portal.Wall = hit.collider;

            portal.PlacePortal();
        }
    }
    public void OnPortalPlaceLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // REFACTOR TO MAKE SPLAD DO PLACE TOO, NOT RETURN BOOL
            SetPortalLocationAndDirection(leftPortal);
        }
    }
    
    public void OnPortalPlaceRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SetPortalLocationAndDirection(rightPortal);
        }
    }
    
    public void OnPortalRemoveLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            leftPortal.RemovePortal();
        }
    }
    
    public void OnPortalRemoveRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rightPortal.RemovePortal();
        }
    }

    // 

    /**
     * Offset 0.01 if hits front, -1.01 if hits back
     * https://answers.unity.com/questions/473871/detect-side-of-collision.html
     */
    private float PortalSideCheck(Vector3 hitNormal, Vector3 portalWallForward)
    {
        float angle = Vector3.Angle(hitNormal, portalWallForward);
        if (Mathf.Approximately(angle, 180)) return -1.01f;
        if (Mathf.Approximately(angle, 0)) return 0.01f;
        return 0.0f;
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