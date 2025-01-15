using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class FractalCuller : MonoBehaviour {
    public bool IsInCameraView(Camera camera) {
        Vector3 viewportPos = camera.WorldToViewportPoint(transform.position);
        // if (camera.cameraType == CameraType.SceneView) {
        //     Debug.Log(viewportPos);
        // }
        if (viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1 && viewportPos.z > 0)
        {
            return true;
        }else{
            return false;
        }
    }
}