using UnityEngine;

public class RotateAroundAxis : MonoBehaviour {
    public Vector3 axis = Vector3.up;
    public GameObject rootedObject;
    public float speed = 50f;

    private void Start() {
        rootedObject = new GameObject("RotateAroundAxisRootedObject");
    }

    private void Update() {
        rootedObject.transform.Rotate(axis, speed * Time.deltaTime);
        transform.rotation = rootedObject.transform.rotation;
    }
}