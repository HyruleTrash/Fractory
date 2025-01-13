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
    public bool autoSpawn = false;
    public float autoSpawnInterval = 2f;
    public bool giveInteractPopUp = false;
    public GameObject interactPopUpPrefab;
    public Vector3 interactPopUpOffset = new Vector3(0, 2.5f, 0);
    private float autoSpawnTimer = 0f;

    private void Start() {
        if (levelButton != null) {
            levelButton.buttonPressedListeners.Add(ButtonPressed);
        }
        if (conveyor != null) {
            conveyor.onConveyorFinishedListeners.Add(ConveyorFinished);
        }
    }

    private void Update() {
        if (autoSpawn && spawnedFractals.Count < maxSpawnAmount) {
            autoSpawnTimer += Time.deltaTime;
            if (autoSpawnTimer >= autoSpawnInterval) {
                autoSpawnTimer = 0f;
                SpawnFractal();
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(spawnPoint.position, 0.2f);
        if (giveInteractPopUp){
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(spawnPoint.position + interactPopUpOffset, 0.2f);
        }
    }

    private void ButtonPressed(string tag, GameObject button) {
        SpawnFractal();
    }

    private void ConveyorFinished(GameObject conveyor) {
        SpawnFractal();
    }

    public void SpawnFractal() {
        if (spawnedFractals.Count >= maxSpawnAmount) {
            return;
        }

        GameObject fractal = Instantiate(fractalPrefab, spawnPoint.position, Quaternion.identity);
        fractal.AddComponent<SpawnerTracker>().spawner = this;
        if (giveInteractPopUp) {
            InteractPopUpRegistry registry = fractal.AddComponent<InteractPopUpRegistry>();
            GameObject interactPopUp = Instantiate(interactPopUpPrefab, fractal.transform.position + interactPopUpOffset, interactPopUpPrefab.transform.rotation, fractal.transform);
            interactPopUp.GetComponent<InteractPopUp>().player = GameObject.FindGameObjectWithTag("Player");
            interactPopUp.GetComponent<InteractPopUp>().meshRenderer = interactPopUp.GetComponentInChildren<MeshRenderer>();
            interactPopUp.GetComponent<InteractPopUp>().distanceThreshold = 5f;
            registry.interactPopUp = interactPopUp.GetComponent<InteractPopUp>();
        }
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
            conveyor.onConveyorFinishedListeners.Remove(ConveyorFinished);
        }
    }
}