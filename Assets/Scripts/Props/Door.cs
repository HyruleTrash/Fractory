using UnityEngine;

public class Door : MonoBehaviour {
    public LevelButton levelButton;
    public Conveyor conveyor;
    public Animator animator;

    private void Start() {
        if (levelButton != null) {
            levelButton.buttonPressed = ButtonPressed;
            levelButton.unButtonPressed = UnButtonPressed;
        }
        if (conveyor != null) {
            conveyor.onConveyorFinished = Close;
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
}