using UnityEngine;

public class ConveyorDisplay : Display {

    [SerializeField]
    public FractalRenderer requiredFractal;

    public GameObject extraUI;

    protected override void AfterInteract()
    {
        extraUI.SetActive(true);
    }

    protected override void AfterTurnOff()
    {
        extraUI.SetActive(false);
    }
}