using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Small helper shared by everything that reacts to screen taps.
public static class ScreenInput
{
    static readonly List<RaycastResult> uiHits = new();

    // true when the finger is on a button or panel, so the tap shouldn't also shoot or place things
    public static bool IsOverUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;

        var data = new PointerEventData(EventSystem.current) { position = screenPos };
        uiHits.Clear();
        EventSystem.current.RaycastAll(data, uiHits);
        return uiHits.Count > 0;
    }
}
