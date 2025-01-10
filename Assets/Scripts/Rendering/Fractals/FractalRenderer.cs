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
        #if UNITY_EDITOR
        if (manager == null && UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null && gameObject.scene.name != null) {
            Debug.LogError("FractalRenderer can only be used if a FractalManager exists in the scene.");
        }
        #endif
    }

    private void Start() {
        manager = FindFirstObjectByType<FractalManager>();
        if (manager == null) {
            Debug.LogError("FractalRenderer can only be used if a FractalManager exists in the scene.");
        }
        manager.fractalRenderers.Add(this);
    }

    private void OnDestroy() {
        if (manager == null) {
            return;
        }
        manager.fractalRenderers.Remove(this);
    }
}