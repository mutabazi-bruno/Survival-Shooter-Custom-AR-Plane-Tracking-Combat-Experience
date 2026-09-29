using UnityEngine;

// Base class for every screen (menu, HUD, game over...).
// Handles showing and hiding with a quick fade and a small scale-in,
// so each panel only has to fill in its own content in OnShow.
[RequireComponent(typeof(CanvasGroup))]
public abstract class UIPanel : MonoBehaviour
{
    [SerializeField] float fadeTime = 0.25f;
    [SerializeField] float hiddenScale = 0.96f;

    CanvasGroup group;
    float targetAlpha;

    // panels can be shown before their Awake has run, so grab this when first needed
    CanvasGroup Group => group != null ? group : group = GetComponent<CanvasGroup>();

    public bool IsVisible => targetAlpha > 0f;

    public void Show()
    {
        gameObject.SetActive(true);
        targetAlpha = 1f;
        Group.interactable = true;
        Group.blocksRaycasts = true;
        OnShow();
    }

    public void Hide(bool instant = false)
    {
        targetAlpha = 0f;
        Group.interactable = false;
        Group.blocksRaycasts = false;

        if (instant)
        {
            Group.alpha = 0f;
            gameObject.SetActive(false);
        }
        OnHide();
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }

    protected virtual void Update()
    {
        if (Mathf.Approximately(Group.alpha, targetAlpha)) return;

        // unscaled so fades still work if we ever pause with timeScale
        Group.alpha = Mathf.MoveTowards(Group.alpha, targetAlpha, Time.unscaledDeltaTime / fadeTime);
        transform.localScale = Vector3.one * Mathf.Lerp(hiddenScale, 1f, Group.alpha);

        if (Group.alpha <= 0f)
            gameObject.SetActive(false);
    }
}
