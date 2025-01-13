using UnityEngine;

public class Door : MonoBehaviour {
    public LevelButton levelButton;
    public Conveyor conveyor;
    public Animator animator;

    private void Start() {
        if (levelButton != null) {
            levelButton.buttonPressedListeners.Add(ButtonPressed);
            levelButton.unButtonPressedListeners.Add(UnButtonPressed);
        }
        if (conveyor != null) {
            conveyor.onConveyorFinishedListeners.Add(Open);
        }
    }

    private void ButtonPressed(string tag) {
        Open();
    }
    private void UnButtonPressed(string tag) {
        Close();
    }

    public void Open(){
        animator.SetTrigger("DoorOpen");
    }

    public void Close(){
        animator.SetTrigger("DoorClose");
    }

    private void OnDestroy() {
        if (levelButton != null) {
            levelButton.buttonPressedListeners.Remove(ButtonPressed);
            levelButton.unButtonPressedListeners.Remove(UnButtonPressed);
        }
        if (conveyor != null) {
            conveyor.onConveyorFinishedListeners.Remove(Open);
        }
    }
}