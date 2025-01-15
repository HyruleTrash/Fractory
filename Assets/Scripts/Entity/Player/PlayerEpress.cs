using UnityEngine;

public class PlayerEpress : MonoBehaviour {
    virtual public float GetDistance() {
        return Mathf.Infinity;
    }
    virtual public float GetLookAt() {
        return -1;
    }

    virtual public void OnPress() {
    }
}