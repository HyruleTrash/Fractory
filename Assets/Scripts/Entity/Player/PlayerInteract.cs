using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : PlayerEpress {
    public delegate void OnInteract();
    public OnInteract onInteract;
    public GameObject nearestInteractable;
    public float interactDistance = 2;
    private float distance;
    
    private void Update() {
        if (nearestInteractable != null)
        {
            distance = MathUtil.GetDistanceXZ(transform.position, nearestInteractable.transform.position);
            if (distance > interactDistance)
            {
                CancleInteract();
            }
        }   
    }

    private void CancleInteract() {
        nearestInteractable.GetComponent<Display>()?.TurnOff();
        nearestInteractable = null;
        onInteract = null;
    }

    public override void OnPress() {
        if (nearestInteractable != null)
        {
            Display display = nearestInteractable.GetComponent<Display>();
            if (display != null && display.isRendering)
            {
                CancleInteract();
            }else if (display != null){
                display.OnInteract();
            }
        }
    }

    public override float GetDistance() {
        if (nearestInteractable == null)
        {
            return Mathf.Infinity;
        }

        distance = MathUtil.GetDistanceXZ(transform.position, nearestInteractable.transform.position);
        if (distance <= interactDistance)
        {
            return distance;
        }else{
            return Mathf.Infinity;
        }
    }

    public override float GetLookAt() {
        if (nearestInteractable == null)
        {
            return -1;
        }

        Vector3 localPos = (nearestInteractable.transform.position - transform.position).normalized;
        return Vector3.Dot(transform.forward, localPos);
    }
}