using System;

// This struct type is used for defining a minimum and maximum value
[Serializable]
public struct MinMax
{
    public float min;
    public float max;

    public MinMax(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}