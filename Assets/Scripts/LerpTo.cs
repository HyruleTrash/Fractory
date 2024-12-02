using UnityEngine;

public class LerpTo : MonoBehaviour {
    public Transform target;
    public float speed = 1f;

    private void Start() {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    private void Update() {
        transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
    }

    private void OnDestroy() {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
    }
}