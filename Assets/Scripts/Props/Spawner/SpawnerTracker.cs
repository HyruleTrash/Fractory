using UnityEngine;

public class SpawnerTracker : MonoBehaviour {
    public FractalSpawner spawner;

    private void OnDestroy() {
        if (spawner == null) {
            return;
        }
        spawner.RemoveFractal(this.gameObject);
    }
}