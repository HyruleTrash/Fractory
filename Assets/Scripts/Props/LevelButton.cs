using System.Collections.Generic;
using System.Linq;
using UnityEditor.EditorTools;
using UnityEngine;

public class LevelButton : MonoBehaviour {
    [SerializeField]
    public delegate void ButtonPressed(string tag);
    [SerializeField]
    public delegate void UnButtonPressed(string tag);

    private class ButtonData {
        public string tag;
        public bool isPressed;
    }

    [Tooltip("The tags a collider requires to let the button become pressed.")]
    public string[] requiredTags;
    public bool isPressed = false;
    public ButtonPressed buttonPressed;
    public UnButtonPressed unButtonPressed;

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
            buttonPressed?.Invoke(data.tag);
        } else if (isPressed == true && !isTriggered && !data.isPressed) {
            materials[1] = OffMaterial;
            GetComponent<MeshRenderer>().SetMaterials(materials);
            isPressed = false;
            unButtonPressed?.Invoke(data.tag);
        }
    }

    private ButtonData HasCorrectTags(){
        foreach (Collider collider in triggerTracker.colliders) {
            if (requiredTags.Contains(collider.tag)) {
                return new ButtonData { tag = collider.tag, isPressed = true };
            }
        }
        return new ButtonData { tag = "", isPressed = false };
    }
}