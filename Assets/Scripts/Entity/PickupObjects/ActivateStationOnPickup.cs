using UnityEngine;

public class ActivateStationOnPickup : MonoBehaviour {
    public FractalStation station;
    public FractalSpawner spawner;

    public void Activate() {
        if (station != null) {
            station.UnLock();
            spawner.turnOnStationOnPickup = false;
            Destroy(this);
        }
    }
}