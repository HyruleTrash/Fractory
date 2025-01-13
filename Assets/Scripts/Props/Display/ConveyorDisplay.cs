using UnityEngine;

public class ConveyorDisplay : Display {

    [SerializeField]
    public FractalRenderer requiredFractal;

    public GameObject extraUI;

    protected override void AfterInteract()
    {
        if (extraUI)
            extraUI.SetActive(true);
    }

    protected override void AfterTurnOff()
    {
        if (extraUI)
            extraUI.SetActive(false);
    }
}