using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FractalManager : GlobalMonoBehaviour {
    [HideInInspector]
    public List<FractalRenderer> fractalRenderers = new List<FractalRenderer>();
    private Fractal[] fractals;

    private void Awake() {
        fractalRenderers = new List<FractalRenderer>(transform.root.GetComponentsInChildren<FractalRenderer>());
    }

    public Fractal[] GetFractals(Camera camera) {
        List<Fractal> fractals = new List<Fractal>();

        for (int i = 0; i < fractalRenderers.Count; i++) {
            FractalRenderer renderer = fractalRenderers[i];
            if (renderer.GetComponent<FractalCuller>().IsInCameraView(camera)) {
                Fractal fractal = new Fractal();
                fractal.position = renderer.transform.position;
                fractal.rotation = Matrix4x4.TRS(Vector3.zero, renderer.transform.rotation, Vector3.one);
                fractal.scale = renderer.transform.localScale;
                fractal.color = new Vector3(renderer.color.r, renderer.color.g, renderer.color.b);
                fractal.type = (float)renderer.type;
                fractal.bevel = renderer.bevel;
                fractal.complexity = renderer.complexity;
                fractals.Add(fractal);
            }
        }

        return fractals.ToArray();
    }
}