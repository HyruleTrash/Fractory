using UnityEngine;

[ExecuteAlways]
public class PositionOffsetter : MonoBehaviour {
    public Vector3 offset;
    [HideInInspector]
    public Vector3 origin;
    public bool running = true;
    public bool resetOrigin;

    private void Start() {
        origin = transform.position;
    }

    private void OnValidate() {
        if (resetOrigin) {
            origin = transform.position;
            resetOrigin = false;
        }
    }

    private void Update() {
        if (!running) return;
        transform.position = origin + offset;
    }
}