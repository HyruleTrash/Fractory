using UnityEngine;

public class ConveyorDisplay : MonoBehaviour {

    [SerializeField]
    public FractalRenderer requiredFractal;
    public GameObject fractal;
    public TriggerTracker triggerTracker;

    private void Update() {
        PlayerInteract playerInteract = triggerTracker.GetColliderWithClass<PlayerInteract>();
        if (playerInteract != null)
        {
            playerInteract.onInteract = OnInteract;
            playerInteract.nearestInteractable = gameObject;
        }
    }

    public void OnInteract(){
        Debug.Log("Interacted with Fractal Display");
    }
}