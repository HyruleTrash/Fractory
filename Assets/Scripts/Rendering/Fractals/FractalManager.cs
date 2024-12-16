using System.Collections.Generic;
using UnityEngine;

public class FractalManager : GlobalMonoBehaviour {
    public List<FractalRenderer> fractalRenderers = new List<FractalRenderer>();

    public Fractal[] GetFractals() {
        Fractal[] fractals = new Fractal[fractalRenderers.Count];

        for (int i = 0; i < fractalRenderers.Count; i++) {
            FractalRenderer renderer = fractalRenderers[i];
            Fractal fractal = new Fractal();
            fractal.position = renderer.transform.position;
            fractal.rotation = renderer.transform.eulerAngles;
            fractal.scale = renderer.transform.localScale;
            fractal.type = renderer.type;
            fractals[i] = fractal;
        }

        return fractals;
    }
}