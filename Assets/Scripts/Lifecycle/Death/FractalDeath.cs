using UnityEngine;

public class FractalDeath : MonoBehaviour {
    public void DeathKeepInSpawnLimit() {
        gameObject.GetComponent<SpawnerTracker>().active = false;
        Destroy(gameObject);
    }
}