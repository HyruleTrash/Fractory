using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Playermotion : MonoBehaviour
{
    public float horizontalSpeed = 5f;
    public float gravity = 9.81f;
    public float jumpForce = 10f;
    public float intersectionCheckDistance = 0.1f;
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update() 
    {
        HorizontalMotion();
        VerticalMotion();
    }

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

        controller.Move(motion * Time.fixedDeltaTime); // Move the player
    }

    private void VerticalMotion()
    {
        Vector3 motion = Vector3.down * gravity * (controller.isGrounded ? 0 : 1); // Apply gravity

        Debug.Log(controller.isGrounded);

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            Debug.Log("Jumping");
            motion += Vector3.up * jumpForce; // Apply jump force
        }
        controller.Move(motion * Time.fixedDeltaTime); // Move the player
    }
}
