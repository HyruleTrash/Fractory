using UnityEngine;
using System;

public static class HierarchyUtil
{
    // Loops through children in search fo the first child that has a certain type.
    // This function does not search through the children of children.
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
