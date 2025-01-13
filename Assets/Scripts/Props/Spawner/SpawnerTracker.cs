using UnityEngine;

public class SpawnerTracker : MonoBehaviour {
    public FractalSpawner spawner;
    public bool active = true;

    private void OnDestroy() {
        if (spawner == null || !active) {
            return;
        }
        spawner.RemoveFractal(this.gameObject);
    }
}