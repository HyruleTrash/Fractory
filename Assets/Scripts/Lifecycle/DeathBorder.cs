using UnityEngine;

public class DeathBorder : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Component[] components = other.gameObject.GetComponents<Component>();
        foreach (Component component in components) {
            if (component.GetType().GetMethod("Death") != null) {
                component.GetType().GetMethod("Death").Invoke(component, null);
            }
        }
    }
}