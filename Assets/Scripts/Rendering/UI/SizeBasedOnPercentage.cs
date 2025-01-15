using UnityEngine;

[ExecuteAlways]
public class SizeBasedOnPercentage : MonoBehaviour {
    private RectTransform rectTransform;
    public bool setting = false;
    public Vector2 percentage;
    public bool equalToWidth = false;
    public bool equalToHeight = false;
    public bool OnlyDoWidth = false;
    public bool OnlyDoHeight = false;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update() {
        Vector2 size = new Vector2(Screen.width * (percentage.x / 100), Screen.height * (percentage.y / 100));
        if (setting) {
            if (OnlyDoWidth) {
                rectTransform.sizeDelta = new Vector2(size.x, rectTransform.sizeDelta.y);
            }else if (OnlyDoHeight) {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, size.y);
            }else if (equalToWidth) {
                rectTransform.sizeDelta = new Vector2(size.x, size.x);
            }else if (equalToHeight) {
                rectTransform.sizeDelta = new Vector2(size.y, size.y);
            }else{
                rectTransform.sizeDelta = new Vector2(size.x, size.y);
            }
        }
    }
}