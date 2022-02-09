using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class AnimationStateController : MonoBehaviour
{
    [SerializeField] Animator animator;

    private static readonly int _isWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int _isSprintingHash = Animator.StringToHash("isSprinting");
    private static readonly int _isCrouchingHash = Animator.StringToHash("isCrouching");
    private static readonly int _isAttackingHash = Animator.StringToHash("isAttacking");


    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void animateWalk(InputAction.CallbackContext context)
    {
        if (context.performed) animator.SetBool(_isWalkingHash, true);
        else if (context.canceled) animator.SetBool(_isWalkingHash, false);
    }
    
    public void animateRun(InputAction.CallbackContext context)
    {
        if (context.performed) animator.SetBool(_isSprintingHash, true);
        else if (context.canceled) animator.SetBool(_isSprintingHash, false);
    }
    
    public void animateCrouch(InputAction.CallbackContext context)
    {
        if (context.performed) animator.SetBool(_isCrouchingHash, true);
        else if (context.canceled) animator.SetBool(_isCrouchingHash, false);;
    }
    
    public void animateAttack(InputAction.CallbackContext context)
    {
        if (context.performed) animator.SetTrigger(_isAttackingHash);
        else if (context.canceled) animator.ResetTrigger(_isAttackingHash);
    }
    
    
}