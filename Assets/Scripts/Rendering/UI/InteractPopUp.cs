using UnityEngine;

public class InteractPopUp : MonoBehaviour {
    public GameObject player;
    public MeshRenderer meshRenderer;
    public float distanceThreshold = 2;

    private void Update() {
        if (player == null)
        {
            return;
        }
        if (Vector3.Distance(transform.position, player.transform.position) < distanceThreshold)
        {
            meshRenderer.enabled = true;
        }else{
            meshRenderer.enabled = false;
        }
    }
}