using UnityEngine;

public class TempCamFrustumTest : MonoBehaviour {

    private void OnDrawGizmos() {
        Camera cam = GetComponent<Camera>();

        Matrix4x4 frustum = MathUtil.CamFrustum(cam);
        Vector3 camPos = cam.transform.position;
        Matrix4x4 rotationM = Matrix4x4.TRS(Vector3.zero, cam.transform.rotation, Vector3.one);

        Gizmos.color = Color.red;
        Vector4 one = frustum.GetRow(0);
        Gizmos.DrawLine(camPos, camPos -  rotationM.MultiplyVector(new Vector3(one.x, one.y, one.z)));
        Vector4 two = frustum.GetRow(1);
        Gizmos.DrawLine(camPos, camPos - rotationM.MultiplyVector(new Vector3(two.x, two.y, two.z)));
        Vector4 three = frustum.GetRow(2);
        Gizmos.DrawLine(camPos, camPos - rotationM.MultiplyVector(new Vector3(three.x, three.y, three.z)));
        Vector4 four = frustum.GetRow(3);
        Gizmos.DrawLine(camPos, camPos - rotationM.MultiplyVector(new Vector3(four.x, four.y, four.z)));
    }
}