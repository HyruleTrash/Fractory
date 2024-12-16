using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// A simple class that holds the logic to keep a gameobject within scene switching, and the only one inside the current context
// !!! Important note, a tag of the same name as the class must be made
public class GlobalMonoBehaviour : MonoBehaviour
{
    void Awake()
    {
        // Make sure there is no other gameobject with this component to be found within the current game context
        this.gameObject.name = this.GetType().ToString();
        this.gameObject.tag = this.gameObject.name;
        GameObject[] objs = GameObject.FindGameObjectsWithTag(this.gameObject.name);

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
