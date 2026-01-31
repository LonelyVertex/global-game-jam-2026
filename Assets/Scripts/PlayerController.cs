using UnityEngine;  
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionReference moveAction;
    public Rigidbody rigidBody;
    public float moveSpeed = 5f;

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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moveAction != null && moveAction.action.enabled)
        {
            Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
            Vector3 moveVector = new Vector3(moveInput.x, 0, moveInput.y);
            rigidBody.MovePosition(rigidBody.position + moveVector * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
