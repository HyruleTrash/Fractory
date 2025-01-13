using UnityEngine;

[ExecuteAlways]
public class FractalRenderer : MonoBehaviour {
    public FractalType type;
    public Color color = Color.white;
    public float bevel;
    [Range(0, 10)]
    public int complexity;
    [HideInInspector]
    public FractalManager manager;
    
    private void OnValidate() {
        manager = FindFirstObjectByType<FractalManager>();
        CheckManager(manager);
    }

    private void CheckManager(FractalManager manager){
        #if UNITY_EDITOR
        if (manager == null && UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null && gameObject.scene.name != null) {
            Debug.LogError("FractalRenderer can only be used if a FractalManager exists in the scene.");
        }
        #endif
    }

    private void AddToManager() {
        manager = FindFirstObjectByType<FractalManager>();
        CheckManager(manager);
        if (manager == null || manager.fractalRenderers.Contains(this)) {
            return;
        }
        manager.fractalRenderers.Add(this);
    }

    private void RemoveFromManager() {
        if (manager == null) {
            return;
        }
        manager.fractalRenderers.Remove(this);
    }

    private void Start() {
        AddToManager();
    }

    private void OnEnable() {
        AddToManager();
    }

    private void OnDestroy() {
        RemoveFromManager();
    }

    private void OnDisable() {
        RemoveFromManager();
    }

    public void CopyTo(GameObject gameObject) {
        FractalRenderer renderer = gameObject.AddComponent<FractalRenderer>();
        renderer.type = type;
        renderer.color = color;
        renderer.bevel = bevel;
        renderer.complexity = complexity;
    }
}