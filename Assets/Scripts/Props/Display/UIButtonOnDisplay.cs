using UnityEngine;

public class UIButtonOnDisplay : MonoBehaviour {
    public GameObject display;
    public Vector3 offset;
    public Camera MainCamera;

    private void Start() {
        MainCamera = Camera.main;
    }

    private void Update() {
        SetPosition();
    }

    public void SetPosition(){
        if (display == null || MainCamera == null)
        {
            return;
        }
        Vector3 targetPosition = MainCamera.WorldToScreenPoint(display.transform.position + (-display.transform.right * offset.x) + (-display.transform.forward * offset.y));
        targetPosition.z = offset.z;
        transform.position = targetPosition;
    }
}