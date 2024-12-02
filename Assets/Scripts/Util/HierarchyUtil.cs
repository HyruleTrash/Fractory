using UnityEngine;
using System;

public static class HierarchyUtil
{
    /// <summary>
    /// Get the first child with a certain component
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="ComponentType"></param>
    /// <returns></returns>
    public static GameObject GetFirstChildWithComponent(Transform transform, Type ComponentType)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent(ComponentType)) {
                return transform.GetChild(i).gameObject;
            }
        }
        return null;
    }
}
