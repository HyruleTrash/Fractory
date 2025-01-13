using UnityEngine;

public class DoorCloseTrigger : MonoBehaviour {
    public Door door;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && door && door.IsOpen()) {
            door.Close();
        }
    }
}