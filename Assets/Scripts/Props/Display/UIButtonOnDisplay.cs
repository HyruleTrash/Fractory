using UnityEngine;

public class UIButtonOnDisplay : MonoBehaviour {
    public GameObject display;
    public Vector3 offset;
    public Camera MainCamera;


    private void Update() {
        if (display == null || MainCamera == null)
        {
            return;
        }
        Vector3 targetPosition = MainCamera.WorldToScreenPoint(display.transform.position) + new Vector3(offset.x, offset.y, 0);
        targetPosition.z = offset.z;
        transform.position = targetPosition;
    }
}