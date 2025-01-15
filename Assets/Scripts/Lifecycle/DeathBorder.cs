using UnityEngine;

public class DeathBorder : MonoBehaviour {
    public bool isGreaterDeath = false;
    private void OnTriggerEnter(Collider other) {
        Component[] components = other.gameObject.GetComponents<Component>();
        foreach (Component component in components) {
            if (!isGreaterDeath){
                if (component.GetType().GetMethod("Death") != null) {
                    component.GetType().GetMethod("Death").Invoke(component, null);
                }
            }else{
                if (component.GetType().GetMethod("GreaterDeath") != null) {
                    component.GetType().GetMethod("GreaterDeath").Invoke(component, null);
                }
            }
        }
    }
}