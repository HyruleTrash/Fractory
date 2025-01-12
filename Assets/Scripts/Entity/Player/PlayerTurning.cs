using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurning : MonoBehaviour {
    public LayerMask layerMask;
    private Vector3 LastRotation;

    private void Update() {
        Gamepad gamepad = Gamepad.current;
        if (gamepad != null){
            Turning(gamepad);
        }else{
            Turning();
        }
    }

    /// <summary>
    /// Turn the player using the mouse
    /// </summary>
    private void Turning(){
        // Send raycast from mouse position to the ground
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = Camera.main.nearClipPlane; // Set Z to near clip plane
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(screenPoint);

        RaycastHit hit;
        if (Physics.Raycast(mousePosition, Camera.main.transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 lookAt = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookAt);
        }
    }

    /// <summary>
    /// Turn the player using the gamepad
    /// </summary>
    /// <param name="gamepad"></param>
    private void Turning(Gamepad gamepad){
        void LookAt(Vector3 rotation){
            Vector3 lookAt = transform.position + rotation;
            transform.LookAt(lookAt);
        }

        Vector2 rightStickValue = gamepad.rightStick.ReadUnprocessedValue();
        Vector3 rotation = new Vector3(
            rightStickValue.x,
            0,
            rightStickValue.y
        ).normalized;
        
        float angle = -45f; // this angle represents the rotation of the isometric view
        rotation = Quaternion.Euler(0, angle, 0) * rotation; // Rotate to fit the icometric view

        Vector2 deadZone = new Vector2(0.01f, 0.01f);
        if (rightStickValue.magnitude > deadZone.magnitude)
        {
            LookAt(rotation);
            LastRotation = rotation;
        }else{
            LookAt(LastRotation);
        }
    }
}