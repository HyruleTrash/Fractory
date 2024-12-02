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
}
