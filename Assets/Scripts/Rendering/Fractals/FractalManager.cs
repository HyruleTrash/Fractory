using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FractalManager : GlobalMonoBehaviour {
    [HideInInspector]
    public List<FractalRenderer> fractalRenderers = new List<FractalRenderer>();

    private void Awake() {
        fractalRenderers = new List<FractalRenderer>(GetComponentsInChildren<FractalRenderer>());
    }

    public Fractal[] GetFractals() {
        Fractal[] fractals = new Fractal[fractalRenderers.Count];

        for (int i = 0; i < fractalRenderers.Count; i++) {
            FractalRenderer renderer = fractalRenderers[i];
            Fractal fractal = new Fractal();
            fractal.position = renderer.transform.position;
            fractal.rotation = renderer.transform.eulerAngles * Mathf.Deg2Rad;
            // fractal.rotation = new Vector3(fractal.rotation.x % (2 * Mathf.PI), fractal.rotation.y % (2 * Mathf.PI), fractal.rotation.z % (2 * Mathf.PI));
            fractal.scale = renderer.transform.localScale;
            fractal.type = renderer.type;
            fractals[i] = fractal;
        }

        return fractals;
    }
}