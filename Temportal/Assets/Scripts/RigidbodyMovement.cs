using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyMovement : MonoBehaviour
{
    // https://www.youtube.com/watch?v=b1uoLBp2I1w (How to Create Player Movement in UNITY (Rigidbody & Character Controller)
    // https://www.youtube.com/watch?v=AwJbOJMFfAg (How To: Make a Rigidbody Movement Controller in Unity3D Part 1)
    // https://www.youtube.com/watch?v=LqnPeqoJRFY (Rigidbody FPS Controller Tutorial #1 | Basic Movement in less than 4 min)

    // Unity Input System HOW TO
    // https://www.youtube.com/watch?v=5tOOstXaIKE (Controlling Cross-Platform Characters with Unity Input System | Tutorial)

    // https://www.youtube.com/watch?v=bwFLLnhm4D4 (Unity 2020 | Rigidbody FPS Movement Tutorial | Pt. 1)

    [Header("Player Components")] 
    [SerializeField] private Transform Camera;
    [SerializeField] private Rigidbody Body;

    [Space(5)]
    [Header("Player Variables")]
    [SerializeField] private float walkSpeed = 6f;

    [SerializeField] private float sprintMultiplier = 1.25f;
    //[SerializeField] private float maxSprintSpeed = 8f;

    [SerializeField] private float crouchMultiplier = 0.75f;
    //[SerializeField] private float maxCrouchSpeed = 4f;

    [SerializeField] private float sensitivity = 1f; // Mouse sens
    [SerializeField] private float jumpForce = 15f;

    private float speed;
    private bool sprinting = false;
    private bool crouching = false;

    private bool grounded = false;

    private Vector3 playerMovementInput;
    private Vector2 playerMouseInput;

    private Vector3 moveDirection;


    void Update()
    {
        //playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        //playerMovementInput = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }


    private void UpdateMovement()
    {
        moveDirection = transform.TransformDirection(playerMovementInput);
        Body.AddForce(playerMovementInput * speed * 10, ForceMode.Acceleration); // * first part by Time.fixedDeltaTime
        Body.velocity = Body.velocity.magnitude > 0.001 ? Vector3.ClampMagnitude(Body.velocity, speed) : new Vector3(0f, 0f, 0f);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 inputMovement = context.ReadValue<Vector2>();
        playerMovementInput = new Vector3(inputMovement.x, 0f, inputMovement.y);

        speed = walkSpeed;
        speed *= crouching ? crouchMultiplier : sprinting ? sprintMultiplier : 1;

    }

    public void Look(InputAction.CallbackContext context)
    {

    }

    public void Jump(InputAction.CallbackContext context)
    {

    }
}
