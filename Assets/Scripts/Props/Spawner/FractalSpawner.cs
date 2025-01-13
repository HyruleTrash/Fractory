using System.Collections.Generic;
using UnityEngine;

public class FractalSpawner : MonoBehaviour {
    public GameObject fractalPrefab;
    public LevelButton levelButton;
    public Conveyor conveyor;
    private List<GameObject> spawnedFractals = new List<GameObject>();
    public int maxSpawnAmount = 10;
    [SerializeField]
    private Transform spawnPoint;

    private void Start() {
        if (levelButton != null) {
            levelButton.buttonPressedListeners.Add(ButtonPressed);
        }
        if (conveyor != null) {
            conveyor.onConveyorFinishedListeners.Add(SpawnFractal);
        }
    }

    private void ButtonPressed(string tag) {
        SpawnFractal();
    }

    public void SpawnFractal() {
        if (spawnedFractals.Count >= maxSpawnAmount) {
            return;
        }

        GameObject fractal = Instantiate(fractalPrefab, spawnPoint.position, Quaternion.identity);
        fractal.AddComponent<SpawnerTracker>().spawner = this;
        fractal.AddComponent<SimpleDeath>();
        spawnedFractals.Add(fractal);
    }

    public void RemoveFractal(GameObject fractal) {
        spawnedFractals.Remove(fractal);
    }

    private void OnDestroy() {
        if (levelButton != null) {
            levelButton.buttonPressedListeners.Remove(ButtonPressed);
        }
        if (conveyor != null) {
            conveyor.onConveyorFinishedListeners.Remove(SpawnFractal);
        }
    }
}