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
            fractal.rotation = Matrix4x4.TRS(Vector3.zero, renderer.transform.rotation, Vector3.one);
            fractal.scale = renderer.transform.localScale;
            fractal.type = (float)renderer.type;
            fractal.bevel = renderer.bevel;
            fractals[i] = fractal;
        }

        return fractals;
    }
}