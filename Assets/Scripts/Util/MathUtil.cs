using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
    /// <summary>
    /// Turns given angle to a angle within a normal radius of -360 and 360. Then clamps it between a minimum and maximum value
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360f) && (angle <= 360f))
        {
            if (angle < -360f)
            {
                angle += 360f;
            }
            if (angle > 360f)
            {
                angle -= 360f;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }

    /// <summary>
    /// Get's the average value of a List or array type object
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static float GetAverage(List<float> data)
    {
        float average = 0;
        for (int i = 0; i < data.Count; i++)
        {
            average += data[i];
        }
        average /= data.Count;
        return average;
    }

    /// <summary>
    /// Get's the direction from one point to another
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static Vector3 DirectionTowards(Vector3 from, Vector3 to){
        return (to - from).normalized;
    }

    public static Vector3 NormalizeDegrees(Vector3 euler){
        return new Vector3(NormalizeDegree(euler.x), NormalizeDegree(euler.y), NormalizeDegree(euler.z));
    }

    public static float NormalizeDegree(float degree){
        return degree % 360;
    }

    public static Matrix4x4 CamFrustum(Camera cam)
    {
        Matrix4x4 frustum = Matrix4x4.identity;

        if (cam.orthographic)
        {
            Vector3 goUp = Vector3.up * cam.orthographicSize;
            Vector3 goRight = Vector3.right * cam.orthographicSize * cam.aspect;
            
            Vector3 nearClipOffset = Vector3.forward * cam.nearClipPlane;
            Vector3 TL = -goRight + goUp - nearClipOffset;
            Vector3 TR = goRight + goUp - nearClipOffset;
            Vector3 BR = goRight - goUp - nearClipOffset;
            Vector3 BL = -goRight - goUp - nearClipOffset;

            frustum.SetRow(0, TL);
            frustum.SetRow(1, BL);
            frustum.SetRow(2, BR);
            frustum.SetRow(3, TR);
        }else{
            float fov = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

            Vector3 goUp = Vector3.up * fov;
            Vector3 goRight = Vector3.right * fov * cam.aspect;

            Vector3 TL = -Vector3.forward - goRight + goUp;
            Vector3 TR = -Vector3.forward + goRight + goUp;
            Vector3 BR = -Vector3.forward + goRight - goUp;
            Vector3 BL = -Vector3.forward - goRight - goUp;

            frustum.SetRow(0, TL);
            frustum.SetRow(1, BL);
            frustum.SetRow(2, BR);
            frustum.SetRow(3, TR);
        }

        return frustum;
    }
}
