using UnityEngine;

public class ConveyorDisplay : Display {

    [SerializeField]
    public FractalRenderer requiredFractal;
    public Conveyor conveyor;
    public GameObject extraUI;

    protected override void AfterStart()
    {
        conveyor.onConveyorFinishedListeners.Add(OnConveyorFinished);
    }

    private void OnConveyorFinished(GameObject conveyor)
    {
        removeHintObjects();
    }

    protected override void AfterInteract()
    {
        base.AfterInteract();
        if (extraUI)
            extraUI.SetActive(true);
    }

    protected override void AfterTurnOff()
    {
        base.AfterTurnOff();
        if (extraUI)
            extraUI.SetActive(false);
    }
}