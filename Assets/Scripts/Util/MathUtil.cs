using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
    // Turns given angle to a angle within a normal radius of -360 and 360. Then clamps it between a minimum and maximum value
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

    // Get's the average value of a List or array type object
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

    public static Vector3 DirectionTowards(Vector3 from, Vector3 to){
        return (to - from).normalized;
    }
}
