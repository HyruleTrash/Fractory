using UnityEngine;

public class Billboard : MonoBehaviour {
    public Camera mainCamera;

    private void Update() {
        transform.rotation = mainCamera.transform.rotation;
        transform.Rotate(90, 180, 0);
    }
}