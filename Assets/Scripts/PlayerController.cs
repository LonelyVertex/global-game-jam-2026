using UnityEngine;  
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference dashAction;
    public Rigidbody rigidBody;
    public float moveSpeed = 5f;
    public Transform playerModel;
    public float dashDistance = 5f;
    public float dashCooldown = 2f;
    public float dashSpeed = 20f;
    private bool canDash = true;
    private bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;
    private Vector3 dashTarget;
    private Vector3 dashStartPosition;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (moveAction == null)
        {
            Debug.LogError("Move Action Reference is not assigned in the Inspector.");
        }
        else
        {
            moveAction.action.Enable();
        }
        if (dashAction == null)
        {
            Debug.LogError("Dash Action Reference is not assigned in the Inspector.");
        }
        else
        {
            dashAction.action.Enable();
        }      
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moveAction != null && moveAction.action.enabled && !isDashing)
        {
            Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
            Vector3 moveVector = new Vector3(moveInput.x, 0, moveInput.y);
            rigidBody.MovePosition(rigidBody.position + moveVector * moveSpeed * Time.fixedDeltaTime);

            //rotate player model to face movement direction
            if (moveVector != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveVector, Vector3.up);
                playerModel.rotation = Quaternion.Slerp(playerModel.rotation, toRotation, 15f * Time.fixedDeltaTime);
            }
        }
        if (isDashing)
        {
            //User LERP to do the dash movement
            float dashProgress = (Time.time - lastDashTime) * dashSpeed / dashDistance;
            if (dashProgress >= 1f)
            {
                dashProgress = 1f;
                ResetDash();
            } else {
                Vector3 newPosition = Vector3.Lerp(dashStartPosition, dashTarget, dashProgress);
                rigidBody.MovePosition(newPosition);
            }
        }
    }

    void Update() {
        if (dashAction != null && dashAction.action.enabled && rigidBody != null && dashAction.action.WasPressedThisFrame())
        {
            Dash();
        }
    }


    public void Dash()
    {
        if (canDash && Time.time >= lastDashTime + dashCooldown)
        {
            Vector3 dashDirection = playerModel.forward;
            isDashing = true;
            canDash = false;
            lastDashTime = Time.time;

            dashTarget = rigidBody.position + dashDirection * dashDistance;
            dashStartPosition = rigidBody.position;
        }
    }

    void OnCollisionEnter(Collision collision) {
        //Check if collison hat tag Obstacle
        if (collision.gameObject.CompareTag("Obstacle")) {
            //Stop dashing
            ResetDash();
        }
    }

    private void ResetDash()
    {
            isDashing = false;
            canDash = true; 
    }   
}
