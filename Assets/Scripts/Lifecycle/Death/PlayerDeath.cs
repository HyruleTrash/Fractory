using UnityEngine;

public class PlayerDeath : MonoBehaviour {
    [HideInInspector]
    public SpawnPointManager manager;
    
    private void Start() {
        manager = FindFirstObjectByType<SpawnPointManager>();
    }

    public void Death() {
        manager = FindFirstObjectByType<SpawnPointManager>();
        if (manager.currentSpawnPoint != null) {
            GetComponent<PlayerMotion>().ResetMotion(manager.currentSpawnPoint.spawnPosition);
        }else{
            Destroy(gameObject);
        }
    }
}