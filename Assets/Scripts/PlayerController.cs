using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public InputActionReference moveAction;
    public InputActionReference dashAction;
    public Rigidbody rigidBody;
    public Transform playerModel;
    public float dashDistance = 5f;
    public float dashCooldown = 2f;
    public float dashSpeed = 20f;
    private bool canDash = true;
    public bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;
    private Vector3 dashTarget;
    private Vector3 dashStartPosition;
    private LayerMask obstacleLayerMask;
    public float currentDashCooldown;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
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
        obstacleLayerMask = LayerMask.GetMask("Obstacles");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerStats.Instance.IsDead()) return;

        if (moveAction != null && moveAction.action.enabled && !isDashing)
        {
            Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
            Vector3 moveVector = new Vector3(moveInput.x, 0, moveInput.y);
            Move(rigidBody.position + moveVector * PlayerStats.Instance.MovementSpeed * Time.fixedDeltaTime);

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
                ResetDash();
            } else {
                Vector3 newPosition = Vector3.Lerp(dashStartPosition, dashTarget, dashProgress);
                Move(newPosition);
            }
        }
    }

    void Update()
    {
        if (canDash && currentDashCooldown > 0.0f)
        {
            currentDashCooldown -= Time.deltaTime;
        }

        if (dashAction != null && dashAction.action.enabled && rigidBody != null && dashAction.action.WasPressedThisFrame())
        {
            Dash();
        }
    }


    public void Dash()
    {
        if (canDash && currentDashCooldown <= 0.0f)
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
            if (isDashing)
            {
                ResetDash();
            }
        }
    }

    private void ResetDash()
    {
        isDashing = false;
        canDash = true;
        currentDashCooldown = dashCooldown;
    }

    private void Move(Vector3 newPostition)
    {
        //use raycast to check new postion if there is anythong in the way
        RaycastHit hit;
        var direction = (newPostition - rigidBody.position).normalized;
        if (!Physics.Raycast(rigidBody.position, direction, out hit, Vector3.Distance(rigidBody.position, newPostition),obstacleLayerMask))
        {
            rigidBody.MovePosition(newPostition);
        } else {
            var updatedPostion = hit.point - direction * 0.5f; //move to just before the hit point
            var newTarget = new Vector3(updatedPostion.x, rigidBody.position.y, updatedPostion.z);
            //move to hit point
            rigidBody.MovePosition(newTarget);
        }
        rigidBody.linearVelocity = Vector3.zero;
    }
}
