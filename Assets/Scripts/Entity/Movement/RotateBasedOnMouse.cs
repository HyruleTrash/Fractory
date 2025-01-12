using UnityEngine;
using UnityEngine.InputSystem;

public class RotateBasedOnMouse : MonoBehaviour {
    public float speed = 10f;
    private Quaternion originRotation;
    private Vector2 addedRotation;

    private void Start() {
        originRotation = transform.rotation;
    }

    private void Update() {
        Gamepad gamepad = Gamepad.current;
        if (gamepad != null){
            Vector2 rightStick = gamepad.rightStick.ReadValue();
            addedRotation += rightStick * speed * Time.deltaTime;
            Vector2 originValues = new Vector2(originRotation.eulerAngles.y, originRotation.eulerAngles.x);
            transform.rotation = Quaternion.Euler(addedRotation.y + originValues.y, -addedRotation.x + originValues.x, 0);
        }else{
            Vector3 mousePos = Input.mousePosition;
            float transformToPercentage(float value, float screenSize){
                value -= screenSize * 0.5f;
                value *= 2;
                value /= screenSize;
                return value;
            }
            Vector2 percentageToScreenSize = new Vector2(
                transformToPercentage(mousePos.x, Screen.width),
                transformToPercentage(mousePos.y, Screen.height)
            );
            percentageToScreenSize *= speed;
            transform.rotation = Quaternion.Euler(percentageToScreenSize.y, -percentageToScreenSize.x, 0);
        }
    }
}