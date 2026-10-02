using UnityEngine;

// For backgrounds that sit inside the SafeArea but should still cover the whole screen.
// Without it the camera feed shows through in the strip behind the status bar and notch.
[RequireComponent(typeof(RectTransform))]
public class FillScreen : MonoBehaviour
{
    RectTransform rect;
    Canvas canvas;
    Rect applied;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>().rootCanvas;
    }

    void Start() => Apply();

    void Update()
    {
        if (Screen.safeArea != applied) Apply();
    }

    void Apply()
    {
        applied = Screen.safeArea;
        float scale = canvas.scaleFactor;
        if (scale <= 0f) return;

        // stretch to the safe area, then push each edge back out to the real screen edge
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = -applied.min / scale;
        rect.offsetMax = new Vector2(Screen.width - applied.xMax, Screen.height - applied.yMax) / scale;
    }
}
