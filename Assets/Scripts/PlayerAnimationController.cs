using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    private static readonly int _speed = Animator.StringToHash("Speed");
    private static readonly int _dashing = Animator.StringToHash("Dashing");

    public InputActionReference moveAction;

    public PlayerController playerController;
    public Animator animator;

    private void Update()
    {
        animator.SetFloat(_speed, moveAction.action.ReadValue<Vector2>().magnitude);
        animator.SetBool(_dashing, playerController.isDashing);
    }
}
