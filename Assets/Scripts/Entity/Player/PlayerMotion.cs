using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(StateManager))]
public class PlayerMotion : MonoBehaviour
{
    public float horizontalSpeed = 5f;
    public float gravity = 9.81f;
    public float groundCheckOffset = 0.1f;
    public LayerMask groundLayers;
    private CharacterController controller;
    private StateManager stateManager;
    public enum PlayerMovementState
    {
        Falling
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        stateManager = GetComponent<StateManager>();

        // Add possible states to the state manager
        stateManager.AddPossibleState(new string[] {
            PlayerMovementState.Falling.ToString()
        });
    }

    private void FixedUpdate() 
    {
        HorizontalMotion();
        VerticalMotion();
    }

    /// <summary>
    /// Apply horizontal motion to the player
    /// </summary>
    private void HorizontalMotion()
    {
        // Store user input as a motion vector
        Vector3 motion = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ).normalized;

        float angle = -45f; // this angle represents the rotation of the isometric view
        motion = Quaternion.Euler(0, angle, 0) * motion; // Rotate to fit the icometric view

        motion *= horizontalSpeed; // Apply current motion speed

        if (controller.enabled)
            controller.Move(motion * Time.fixedDeltaTime); // Move the player or apply friction
    }

    /// <summary>
    /// Apply vertical motion to the player
    /// </summary>
    private void VerticalMotion()
    {
        bool isGrounded = IsGrounded();
        Vector3 motion = Vector3.zero;

        // Check if the player is grounded, and remove the falling state if it is
        if (isGrounded)
        {
            stateManager.RemoveState(PlayerMovementState.Falling.ToString());
        }else
        {
            stateManager.AddState(PlayerMovementState.Falling.ToString());
        }

        // Apply gravity if the player is falling
        if (stateManager.HasState(PlayerMovementState.Falling.ToString()))
        {
            motion = Vector3.down * gravity; // Apply gravity
        }

        if (controller.enabled)
            controller.Move(motion * Time.fixedDeltaTime); // Move the player
    }

    public bool IsGrounded()
    {
        Debug.DrawRay(
            transform.position + controller.height / 2 * Vector3.down,
            Vector3.down * groundCheckOffset,
            Color.red
        );
        return Physics.Raycast(
            transform.position + controller.height / 2 * Vector3.down,
            Vector3.down,
            groundCheckOffset,
            groundLayers,
            QueryTriggerInteraction.Ignore
        );
    }

    public void EnableController() => controller.enabled = true;

    public void ResetMotion(Vector3 newPosition)
    {
        controller.Move(controller.velocity * -1);
        controller.enabled = false;
        transform.position = newPosition;
        Invoke("EnableController", 0.1f);
    }
}
