using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Playermotion : MonoBehaviour
{
    public float horizontalSpeed = 5f;
    public float gravity = 9.81f;
    public bool isGrounded = false;
    public LayerMask collisionLayer;
    public float intersectionCheckDistance = 0.1f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void FixedUpdate()
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

        //motion += Vector3.down * gravity * (isGrounded ? 0 : 1); // Apply gravity

        // Apply motion using MovePosition
        rb.MovePosition(CheckCollision(motion * Time.fixedDeltaTime));
    }

    /// <summary>
    /// Returns the found position, and triggers any collision functions when needed
    /// </summary>
    /// <param name="motion"></param>
    /// <returns></returns>
    Vector3 CheckCollision(Vector3 motion){
        RaycastHit hit;
        Vector3 closestPoint = rb.GetComponent<Collider>().ClosestPoint(motion.normalized * 20);
        if (Physics.Raycast(closestPoint, motion.normalized, out hit, motion.magnitude, collisionLayer, QueryTriggerInteraction.Collide)){
            OnCollision(hit);
            Vector3 offset = MathUtil.DirectionTowards(closestPoint, hit.point) * Vector3.Distance(closestPoint, hit.point);
            // DebugUtil.DrawSphere(hit.point, Color.red);
            // DebugUtil.DrawSphere(closestPoint, Color.green);
            // DebugUtil.DrawSphere(rb.position, Color.blue);
            // DebugUtil.DrawSphere(rb.position + offset, Color.yellow);
            // Time.timeScale = 0;
            return rb.position + offset;
        }
        
        return rb.position + motion;
    }

    private void OnCollision(RaycastHit other) {
        Debug.Log("Hit: " + other.collider.name);
    }
}
