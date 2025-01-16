using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayButton : Display {
    [SerializeField]
    public delegate void ButtonPressed(GameObject button);
    public List<ButtonPressed> buttonPressedListeners = new List<ButtonPressed>();
    public Vector3 holdOffset = new Vector3(0, 0, 3);

    protected override void AfterStart(){
        foreach (DisplayUI userInterface in userInterfaces)
        {
            Button button = userInterface.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(Activate);
            }
        }
    }

    public void SetFractal(FractalRenderer fractalRenderer){
        fractal = new GameObject();
        fractal.transform.position = displayCamera.transform.position + holdOffset;
        fractalRenderer.CopyTo(fractal);
        fractal.AddComponent<RotateBasedOnMouse>();
        displayCamera.transform.LookAt(fractal.transform);
    }

    public void RemoveFractal(){
        Destroy(fractal);
    }

    public void Activate(){
        foreach (ButtonPressed listener in buttonPressedListeners)
        {
            listener?.Invoke(gameObject);
            if (fractal != null)
            {
                SetUIText("Iterations: " + fractal.GetComponent<FractalRenderer>().complexity + "\n" + "Rounding: " + fractal.GetComponent<FractalRenderer>().bevel);
            }
        }
    }
}