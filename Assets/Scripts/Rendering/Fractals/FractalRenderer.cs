using UnityEngine;

[ExecuteInEditMode]
public class FractalRenderer : MonoBehaviour {
    public FractalType type;
    [HideInInspector]
    public FractalManager manager;
    
    private void OnValidate() {
        manager = FindFirstObjectByType<FractalManager>();
        if (manager == null) {
            Debug.LogError("FractalRenderer can only be used if a FractalManager exists in the scene.");
        }
    }

    private void Start() {
        manager = FindFirstObjectByType<FractalManager>();
        
        if (manager == null) {
            return;
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