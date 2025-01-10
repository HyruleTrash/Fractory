using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerTracker : MonoBehaviour
{
    public Collider trigger;
    public List<Collider> colliders = new List<Collider>();

    private void Start() {
        trigger = GetComponent<Collider>();
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }
    
    public bool IsTriggered()
    {
        return colliders.Count > 0;
    }
}