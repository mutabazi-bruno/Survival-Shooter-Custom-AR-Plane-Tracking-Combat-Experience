using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

// Shown while the player looks for a floor. Tells them to scan until a plane is found,
// then tells them to tap it.
public class ScanPanel : UIPanel
{
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] TMP_Text hintText;
    [SerializeField] RectTransform phoneIcon;
    [SerializeField] Button backButton;
    [SerializeField] string scanningHint = "MOVE YOUR PHONE SLOWLY TO SCAN THE FLOOR";
    [SerializeField] string readyHint = "TAP THE GRID TO DEPLOY THE ARENA";

    Vector2 iconStart;

    void Awake()
    {
        iconStart = phoneIcon.anchoredPosition;
        backButton.onClick.AddListener(() => GameManager.Instance.GoToMainMenu());
    }

    protected override void Update()
    {
        base.Update();

        bool planeFound = planeManager.trackables.count > 0;
        hintText.text = planeFound ? readyHint : scanningHint;

        // sway the phone icon side to side while scanning, like the motion we want from the player
        phoneIcon.gameObject.SetActive(!planeFound);
        phoneIcon.anchoredPosition = iconStart + Vector2.right * (Mathf.Sin(Time.time * 2.5f) * 40f);
    }
}
