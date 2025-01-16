using System;
using UnityEngine;

[Serializable]
public class DisplayUI : MonoBehaviour {
    public GameObject displayScreen;
    public Vector3 offset;
    public Camera MainCamera;
    [Range(-1, 8)]
    public int index;

    private void Start() {
        MainCamera = Camera.main;
    }

    private void Update() {
        SetPosition();
    }

    public void SetPosition(){
        if (displayScreen == null || MainCamera == null)
        {
            return;
        }
        Vector3 targetPosition = Vector3.zero;
        if (index == -1){
            targetPosition = MainCamera.WorldToScreenPoint(displayScreen.transform.position) + offset;
        }else{
            targetPosition = MainCamera.WorldToScreenPoint(displayScreen.transform.GetChild(index).position) + offset;
        }
        targetPosition.z = offset.z;
        transform.position = targetPosition;
    }
}