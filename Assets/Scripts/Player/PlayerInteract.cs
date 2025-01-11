using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour {

    public delegate void OnInteract();
    public OnInteract onInteract;
    public GameObject nearestInteractable;
    public float interactDistance = 2;
    
    private void Update() {
        Gamepad gamepad = Gamepad.current;
        if (gamepad != null){
            if (gamepad.aButton.wasPressedThisFrame)
            {
                onInteract?.Invoke();
            }
        }else{
            if (Input.GetKeyDown(KeyCode.E))
            {
                onInteract?.Invoke();
            }
        }

        if (nearestInteractable != null)
        {
            if (Vector3.Distance(transform.position, nearestInteractable.transform.position) > interactDistance)
            {
                nearestInteractable = null;
                onInteract = null;
            }
        }
    }
}