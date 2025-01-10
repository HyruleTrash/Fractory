using System.Collections.Generic;
using UnityEngine;

public class FractalSpawner : MonoBehaviour {
    public GameObject fractalPrefab;
    public LevelButton levelButton;
    private List<GameObject> spawnedFractals = new List<GameObject>();
    public int maxSpawnAmount = 10;
    [SerializeField]
    private Transform spawnPoint;

    private void Start() {
        if (levelButton != null) {
            levelButton.buttonPressed = ButtonPressed;
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
        fractal.transform.SetParent(spawnPoint);
        spawnedFractals.Add(fractal);
    }
}