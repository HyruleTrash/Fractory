using System.Collections.Generic;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine;

public class LevelButton : MonoBehaviour {
    [SerializeField]
    public delegate void ButtonPressed(string tag, GameObject button);
    [SerializeField]
    public delegate void UnButtonPressed(string tag, GameObject button);

    private class ButtonData {
        public string tag;
        public bool isPressed;
    }

    [Tooltip("The tags a collider requires to let the button become pressed.")]
    public string[] requiredTags;
    public bool isPressed = false;
    public List<ButtonPressed> buttonPressedListeners = new List<ButtonPressed>();
    public List<UnButtonPressed> unButtonPressedListeners = new List<UnButtonPressed>();

    [SerializeField]
    private Material OnMaterial;
    [SerializeField]
    private Material OffMaterial;

    private TriggerTracker triggerTracker;
    private List<Material> materials;

    private void Start() {
        materials = new List<Material>(GetComponent<MeshRenderer>().materials);
        materials[1] = OffMaterial;
        GetComponent<MeshRenderer>().SetMaterials(materials);
        CheckForRequiredVariables();
    }

    private void OnValidate() {
        CheckForRequiredVariables();
    }

    private void CheckForRequiredVariables() {
        triggerTracker = GetComponent<TriggerTracker>();
        if (triggerTracker == null) {
            Debug.LogError("LevelButton can only be used if a TriggerTracker exists on its object.");
        }
    }

    private void Update() {
        bool isTriggered = triggerTracker.IsTriggered();
        ButtonData data = HasCorrectTags();
        if (isTriggered && data.isPressed && isPressed == false) {
            materials[1] = OnMaterial;
            GetComponent<MeshRenderer>().SetMaterials(materials);
            isPressed = true;
            foreach (ButtonPressed listener in buttonPressedListeners) {
                listener?.Invoke(data.tag, gameObject);
            }
        } else if (isPressed == true && !isTriggered && !data.isPressed) {
            materials[1] = OffMaterial;
            GetComponent<MeshRenderer>().SetMaterials(materials);
            isPressed = false;
            foreach (UnButtonPressed listener in unButtonPressedListeners) {
                listener?.Invoke(data.tag, gameObject);
            }
        }
    }

    private ButtonData HasCorrectTags(){
        foreach (Collider collider in triggerTracker.colliders) {
            if (requiredTags.Contains(collider.tag) && collider.GetComponent<LerpTo>() == null) {
                return new ButtonData { tag = collider.tag, isPressed = true };
            }
        }
        return new ButtonData { tag = "", isPressed = false };
    }
}