using UnityEngine;

public class DisplayScreenGrowShrink : MonoBehaviour {
    public Camera mainCamera;
    public float growShrinkSpeed = 5f;
    public Vector2 distanceOffset = new Vector2(2, 20);
    public float maxSize = 1;
    public float minSize = 0.1f;

    [HideInInspector]
    public bool isMoving = false;
    [HideInInspector]
    public bool directionIsGrow = false;
    private Transform oldParent;
    private Vector3 targetPosition;

    private void Start() {
        if (mainCamera == null) {
            mainCamera = Camera.main;
        }

        Vector3 targetPosition = transform.parent.position + mainCamera.transform.forward * distanceOffset.y;
        transform.position = targetPosition;
        transform.localScale = Vector3.one * minSize;
        isMoving = false;
        directionIsGrow = true;
        GetComponent<MeshRenderer>().material = null;
        GetComponent<MeshRenderer>().enabled = false;

        oldParent = transform.parent;
        transform.parent = null;
    }

    private void Update() {
        if (directionIsGrow)
        {
            targetPosition = mainCamera.transform.position + mainCamera.transform.forward * (mainCamera.nearClipPlane + distanceOffset.x);
            transform.position = Vector3.Lerp(transform.position, targetPosition, growShrinkSpeed);
            if (isMoving){
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * maxSize, growShrinkSpeed);
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f && transform.localScale.magnitude > maxSize - 0.1f)
                {
                    transform.position = targetPosition;
                    transform.localScale = Vector3.one * maxSize;
                    isMoving = false;
                    directionIsGrow = false;
                }
            }
        }
        if (isMoving && !directionIsGrow)
        {
            targetPosition = oldParent.position + mainCamera.transform.forward * distanceOffset.y;
            transform.position = Vector3.Lerp(transform.position, targetPosition, growShrinkSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * minSize, growShrinkSpeed);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f && transform.localScale.magnitude < minSize + 0.1f)
            {
                transform.position = targetPosition;
                transform.localScale = Vector3.one * minSize;
                isMoving = false;
                directionIsGrow = true;
                GetComponent<MeshRenderer>().material = null;
                GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public void Grow(){
        isMoving = true;
        directionIsGrow = true;
    }

    public void Shrink(){
        isMoving = true;
        directionIsGrow = false;
    }
}