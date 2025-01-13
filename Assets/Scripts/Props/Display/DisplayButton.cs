using System.Collections.Generic;
using UnityEngine;

public class DisplayButton : Display {
    [SerializeField]
    public delegate void ButtonPressed(GameObject button);
    public List<ButtonPressed> buttonPressedListeners = new List<ButtonPressed>();
    public UIButtonOnDisplay userInterface;
    public Vector3 holdOffset = new Vector3(0, 0, 3);

    public void SetFractal(FractalRenderer fractalRenderer){
        fractal = new GameObject();
        fractal.transform.position = displayCamera.transform.position + holdOffset;
        fractalRenderer.CopyTo(fractal);
        fractal.AddComponent<RotateBasedOnMouse>();
        displayCamera.transform.LookAt(fractal.transform);
    }

    protected override void AfterInteract(){
        userInterface.gameObject.SetActive(true);
    }

    protected override void AfterTurnOff(){
        userInterface.gameObject.SetActive(false);
    }

    public void RemoveFractal(){
        Destroy(fractal);
    }

    public void Activate(){
        foreach (ButtonPressed listener in buttonPressedListeners)
        {
            listener?.Invoke(gameObject);
        }
    }
}