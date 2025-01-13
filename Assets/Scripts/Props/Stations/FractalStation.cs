using UnityEngine;

public class FractalStation : MonoBehaviour {
    [Tooltip("The tags a collider requires to let the button become pressed.")]
    public string[] requiredTags;
    public Vector3 holdingPosition;
    public LevelButton levelButton;
    public DisplayButton displayButton;
    public Conveyor conveyor;
    private TriggerTracker triggerTracker;

    private void Start() {
        if (levelButton != null) {
            levelButton.buttonPressedListeners.Add(ButtonPressed);
        }
        if (conveyor != null) {
            conveyor.onConveyorFinishedListeners.Add(Activate);
        }
        if (displayButton != null) {
            displayButton.buttonPressedListeners.Add(Activate);
        }
        CheckForRequiredVariables();
    }

    private void OnValidate() {
        CheckForRequiredVariables();
    }

    private void Update() {
        FractalRenderer renderer = triggerTracker.GetColliderWithClass<FractalRenderer>();
        if (triggerTracker.IsTriggered() && renderer != null && renderer.GetComponent<LerpTo>() == null) {
            renderer.transform.position = transform.position + holdingPosition;
            renderer.GetComponent<Rigidbody>().isKinematic = true;
            if (displayButton != null && displayButton.fractal == null) {
                displayButton.SetFractal(renderer);
            }
        }else{
            if (displayButton != null && displayButton.fractal != null) {
                displayButton.RemoveFractal();
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + holdingPosition, 0.2f);
    }

    private void CheckForRequiredVariables() {
        triggerTracker = GetComponent<TriggerTracker>();
        if (triggerTracker == null) {
            Debug.LogError("FractalStation's can only be used if a TriggerTracker exists on its object.");
        }
    }

    private void ButtonPressed(string tag, GameObject button) {
        StationTriggered(tag, triggerTracker);
    }

    private void Activate(GameObject trigger){
        StationTriggered(null, triggerTracker);
    }

    virtual public void StationTriggered(string tag, TriggerTracker triggerTracker) {
        // Nothing on base class
    }

    virtual public void StationFinished() {
        FractalRenderer renderer = triggerTracker.GetColliderWithClass<FractalRenderer>();
        if (displayButton != null && displayButton.fractal != null && renderer != null) {
            displayButton.RemoveFractal();
            displayButton.SetFractal(renderer);
        }
    }

    private void OnDestroy() {
        if (levelButton != null) {
            levelButton.buttonPressedListeners.Remove(ButtonPressed);
        }
        if (conveyor != null) {
            conveyor.onConveyorFinishedListeners.Remove(Activate);
        }
        if (displayButton != null) {
            displayButton.buttonPressedListeners.Remove(Activate);
        }
    }
}