using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    public LevelButton[] levelButtons;
    public Conveyor[] conveyors;
    public Animator animator;
    [HideInInspector]
    public List<GameObject> currentInputs;
    public bool stayOpen;
    private bool multiInput;
    public bool flipState; // If true, the door opens and closes on button press
    private bool isOpen = false; // If true, the door is open
    private bool needsToClose = false;
    private float timeStampClose;
    private float timeToClose = 1f;

    private void Start() {
        if (levelButtons.Length > 1 || conveyors.Length > 1 || (levelButtons.Length == 1 && conveyors.Length == 1)) {
            multiInput = true;
            currentInputs = new List<GameObject>();
        }
        if (levelButtons.Length > 0) {
            foreach (LevelButton levelButtons in levelButtons) {
                levelButtons.buttonPressedListeners.Add(ButtonPressed);
                if (!stayOpen && !flipState) {
                    levelButtons.unButtonPressedListeners.Add(UnButtonPressed);
                }
            }
        }
        if (conveyors.Length > 0) {
            foreach (Conveyor c in conveyors) {
                c.onConveyorFinishedListeners.Add(ConveyorFinished);
            }
        }
    }

    private void Update() {
        if (needsToClose && timeStampClose + timeToClose < Time.time) {
            animator.SetTrigger("DoorClose");
            isOpen = false;
            needsToClose = false;
        }

        if (multiInput) {
            if (!isOpen && currentInputs.Count >= (levelButtons.Length + conveyors.Length) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length) {
                Open();
            }
            if (isOpen && !needsToClose && currentInputs.Count < (levelButtons.Length + conveyors.Length) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length){
                Close();
            }
        }
    }

    private void ButtonPressed(string tag, GameObject button) {
        Open(button);
    }
    private void UnButtonPressed(string tag, GameObject button) {
        Close(button);
    }
    private void ConveyorFinished(GameObject conveyor) {
        Open(conveyor);
    }

    public void Open(GameObject trigger = null){
        if (multiInput && trigger != null) {
            currentInputs.Add(trigger);
        }else{
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length) {
                if (flipState) {
                    if (isOpen) {
                        animator.SetTrigger("DoorClose");
                        isOpen = false;
                        return;
                    }else{
                        animator.SetTrigger("DoorOpen");
                        isOpen = true;
                        return;
                    }
                }else{
                    animator.SetTrigger("DoorOpen");
                    isOpen = true;
                }
            }
        }
    }

    public void Close(GameObject trigger = null){
        if (multiInput && trigger != null && currentInputs.Contains(trigger)) {
            currentInputs.Remove(trigger);
        }else{
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length) {
                timeStampClose = Time.time;
                needsToClose = true;
            }
        }
    }

    private void OnDestroy() {
        if (levelButtons != null) {
            foreach (LevelButton levelButtons in levelButtons) {
                levelButtons.buttonPressedListeners.Remove(ButtonPressed);
                levelButtons.unButtonPressedListeners.Remove(UnButtonPressed);
            }
        }
        if (conveyors != null) {
            foreach (Conveyor c in conveyors) {
                c.onConveyorFinishedListeners.Remove(Open);
            }
        }
    }

    public bool IsOpen() {
        return isOpen;
    }
}