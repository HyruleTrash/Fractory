using UnityEngine;

public class DeathBorder : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Component[] components = other.gameObject.GetComponents<Component>();
        Debug.Log("DeathBorder: " + other.gameObject.name + " entered the death border.");
        foreach (Component component in components) {
            Debug.Log("has component: " + component.GetType().Name);
            if (component.GetType().GetMethod("Death") != null) {
                Debug.Log("DeathBorder: " + other.gameObject.name + " has a Death method");
                component.GetType().GetMethod("Death").Invoke(component, null);
            }
        }
    }
}