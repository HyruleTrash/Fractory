using UnityEngine;

public class FractalStation : MonoBehaviour {
    [Tooltip("The tags a collider requires to let the button become pressed.")]
    public string[] requiredTags;
    public LevelButton levelButton;
    private TriggerTracker triggerTracker;

    private void Start() {
        if (levelButton != null) {
            levelButton.buttonPressed = ButtonPressed;
        }
        CheckForRequiredVariables();
    }

    private void OnValidate() {
        CheckForRequiredVariables();
    }

    private void CheckForRequiredVariables() {
        triggerTracker = GetComponent<TriggerTracker>();
        if (triggerTracker == null) {
            Debug.LogError("FractalStation's can only be used if a TriggerTracker exists on its object.");
        }
    }

    private void ButtonPressed(string tag) {
        StationTriggered(tag, triggerTracker);
    }

    virtual public void StationTriggered(string tag, TriggerTracker triggerTracker) {
        // Nothing on base class
    }
}