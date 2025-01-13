using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour {
    public delegate void OnConveyorFinished();
    public List<OnConveyorFinished> onConveyorFinishedListeners = new List<OnConveyorFinished>();

    public TriggerTracker triggerTracker;
    [SerializeField]
    private float speed;
    [SerializeField]
    private Vector3 direction;
    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private float distance;
    public ConveyorDisplay display;
    [HideInInspector]
    public bool isRunning = false;
    [HideInInspector]
    public bool isFinished = false;
    private GameObject foundFractal;

    private void Update() {
        if (!isFinished && !isRunning)
        {
            foreach (Collider collider in triggerTracker.colliders)
            {
                if (collider.CompareTag("PickUpItem") && collider.GetComponent<LerpTo>() == null)
                {
                    FractalRenderer fractalRenderer = collider.GetComponent<FractalRenderer>();
                    float colorDifference = Vector3.Distance(MathUtil.ColorToVector4(fractalRenderer.color), MathUtil.ColorToVector4(display.requiredFractal.color));
                    if (
                        fractalRenderer != null &&
                        (float)fractalRenderer.type == (float)display.requiredFractal.type && 
                        colorDifference <= 0.1 &&
                        fractalRenderer.bevel == display.requiredFractal.bevel &&
                        fractalRenderer.complexity == display.requiredFractal.complexity
                    )
                    {
                        foundFractal = collider.gameObject;
                        foundFractal.GetComponent<Rigidbody>().isKinematic = true;
                        foundFractal.transform.rotation = Quaternion.identity;
                        foundFractal.transform.position = transform.position + startPosition;
                        isRunning = true;
                    }
                }
            }
        }
        if (isRunning && foundFractal != null)
        {
            if (foundFractal.GetComponent<LerpTo>() != null)
            {
                isRunning = false;
                foundFractal = null;
            }else{
                foundFractal.transform.position = Vector3.Lerp(foundFractal.transform.position, foundFractal.transform.position + direction * distance, speed * Time.deltaTime);
                Vector2 fractalPosition = new Vector2(foundFractal.transform.position.x, foundFractal.transform.position.z);
                Vector2 conveyorPosition = new Vector2(transform.position.x, transform.position.z);
                if (Vector2.Distance(fractalPosition, conveyorPosition) > distance)
                {
                    isRunning = false;
                    isFinished = true;
                    Finish();
                }
            }
        }
    }

    public void Finish(){
        Destroy(foundFractal);
        foreach (OnConveyorFinished listener in onConveyorFinishedListeners)
        {
            listener?.Invoke();
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + startPosition, direction.normalized * distance);
    }
}