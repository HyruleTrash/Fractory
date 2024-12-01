using UnityEngine;
using System;

public static class DebugUtil
{
    /// <summary>
    /// Instantiates a sphere at the given position with the specified color and size
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    /// <param name="size"></param>
    public static void DrawSphere(Vector3 position, Color color, float size = 0.1f) {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * size;
        Renderer renderer = sphere.GetComponent<Renderer>();
        renderer.material.color = color;
    }
}
