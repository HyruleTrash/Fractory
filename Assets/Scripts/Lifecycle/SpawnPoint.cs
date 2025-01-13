using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    [HideInInspector]
    public SpawnPointManager manager;
    public Vector3 spawnPosition;

    private void Start() {
        manager = FindFirstObjectByType<SpawnPointManager>();
        //manager.spawnPoints.Add(this);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(spawnPosition, 0.5f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            manager.currentSpawnPoint = this;
        }
    }
}