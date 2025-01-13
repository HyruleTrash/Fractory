using UnityEngine;

public class InteractPopUpRegistry : MonoBehaviour {
    public InteractPopUp interactPopUp;

    private void OnDestroy() {
        Destroy(interactPopUp.gameObject);
    }
}