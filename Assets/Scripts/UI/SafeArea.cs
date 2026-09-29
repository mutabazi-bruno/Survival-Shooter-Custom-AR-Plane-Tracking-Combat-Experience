using UnityEngine;

// Keeps the UI inside the phone's safe area so nothing hides behind a notch or rounded corner.
[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    RectTransform rect;
    Rect applied;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        Apply();
    }

    void Update()
    {
        // the safe area can change when the phone rotates
        if (Screen.safeArea != applied) Apply();
    }

    void Apply()
    {
        applied = Screen.safeArea;
        if (Screen.width <= 0 || Screen.height <= 0) return;

        rect.anchorMin = new Vector2(applied.xMin / Screen.width, applied.yMin / Screen.height);
        rect.anchorMax = new Vector2(applied.xMax / Screen.width, applied.yMax / Screen.height);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
