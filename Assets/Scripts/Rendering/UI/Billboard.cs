using UnityEngine;

public class Billboard : MonoBehaviour {
    public Camera mainCamera;

    private void Start() {
        if (mainCamera == null) {
            mainCamera = Camera.main;
        }
    }

    private void Update() {
        transform.rotation = mainCamera.transform.rotation;
        transform.Rotate(90, 180, 0);
    }
}