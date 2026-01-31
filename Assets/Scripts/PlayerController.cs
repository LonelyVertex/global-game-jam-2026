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
    }

    void Update() {
        if (dashAction != null && dashAction.action.enabled && rigidBody != null && dashAction.action.WasPressedThisFrame())
        {
            Dash();
        }
    }


    public void Dash()
    {
        Debug.Log("Dash");
        if (canDash && Time.time >= lastDashTime + dashCooldown)
        {
            Vector3 dashDirection = playerModel.forward;
            isDashing = true;
            canDash = false;
            lastDashTime = Time.time;

            Vector3 dashTarget = rigidBody.position + dashDirection * dashDistance;
            StartCoroutine(PerformDash(dashTarget));
        }
    }

    IEnumerator PerformDash(Vector3 dashTarget)
    {
        float startTime = Time.time;
        Vector3 startPosition = rigidBody.position;
        float dashDuration = dashDistance / dashSpeed;

        while (Time.time < startTime + dashDuration)
        {
            float t = (Time.time - startTime) / dashDuration;
            rigidBody.MovePosition(Vector3.Lerp(startPosition, dashTarget, t));
            yield return null;
        }

        rigidBody.MovePosition(dashTarget);
        isDashing = false;

        // Start cooldown timer
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

}
