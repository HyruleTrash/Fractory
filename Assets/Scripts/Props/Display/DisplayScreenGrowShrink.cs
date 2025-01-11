using UnityEngine;

public class DisplayScreenGrowShrink : MonoBehaviour {
    public Camera mainCamera;
    public float growShrinkSpeed = 0.1f;
    public Vector2 distanceOffset = new Vector2(2, 20);
    public float maxSize = 1;
    public float minSize = 0.1f;

    [HideInInspector]
    public bool isGrowing = false;
    [HideInInspector]
    public bool isShrinking = false;

    private void Update() {
        if (isGrowing)
        {
            Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * (mainCamera.nearClipPlane + distanceOffset.x);
            transform.position = Vector3.Lerp(transform.position, targetPosition, growShrinkSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * maxSize, growShrinkSpeed);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f && transform.localScale.magnitude > maxSize - 0.1f)
            {
                transform.position = targetPosition;
                transform.localScale = Vector3.one * maxSize;
                isGrowing = false;
            }
        }else if (isShrinking)
        {
            Vector3 targetPosition = transform.parent.position + mainCamera.transform.forward * distanceOffset.y;
            transform.position = Vector3.Lerp(transform.position, targetPosition, growShrinkSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * minSize, growShrinkSpeed);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f && transform.localScale.magnitude < minSize + 0.1f)
            {
                transform.position = targetPosition;
                transform.localScale = Vector3.one * minSize;
                isShrinking = false;
                GetComponent<MeshRenderer>().material = null;
                GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    public void Grow(){
        isGrowing = true;
    }

    public void Shrink(){
        isShrinking = true;
    }
}