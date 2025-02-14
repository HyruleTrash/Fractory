using UnityEngine;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Fractal {
    public Vector4 position;
    public Matrix4x4 rotation;
    public Vector4 scale;
    public Vector3 color;
    public float type;
    public float bevel;
    public int complexity;
}