using UnityEngine;
using UnityEngine.UI;

// Damage feedback: the screen edges flash red when the player gets hit,
// the phone buzzes, and a faint red border stays on while health is low.
public class DamageFlash : MonoBehaviour
{
    [SerializeField] Image vignette;
    [SerializeField, Range(0f, 1f)] float hitAlpha = 0.75f;
    [SerializeField, Range(0f, 1f)] float lowHealthAlpha = 0.3f;
    [SerializeField, Range(0f, 1f)] float lowHealthPercent = 0.3f;
    [SerializeField] float fadeSpeed = 2f;
    [SerializeField] bool vibrate = true;

    float alpha;
    float restingAlpha;

    void OnEnable()
    {
        GameEvents.PlayerDamaged += Flash;
        GameEvents.PlayerHealthChanged += OnHealthChanged;
        GameEvents.StateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        GameEvents.PlayerDamaged -= Flash;
        GameEvents.PlayerHealthChanged -= OnHealthChanged;
        GameEvents.StateChanged -= OnStateChanged;
    }

    void Start() => SetAlpha(0f);

    void Update()
    {
        if (Mathf.Approximately(alpha, restingAlpha)) return;

        alpha = Mathf.MoveTowards(alpha, restingAlpha, fadeSpeed * Time.deltaTime);
        SetAlpha(alpha);
    }

    void Flash()
    {
        alpha = hitAlpha;
        SetAlpha(alpha);

#if UNITY_ANDROID || UNITY_IOS
        if (vibrate) Handheld.Vibrate();
#endif
    }

    void OnHealthChanged(int current, int max)
    {
        bool lowHealth = current > 0 && current <= max * lowHealthPercent;
        restingAlpha = lowHealth ? lowHealthAlpha : 0f;
    }

    // clear the red once the round is over
    void OnStateChanged(GameStateId state)
    {
        if (state == GameStateId.Playing) return;
        restingAlpha = 0f;
        alpha = 0f;
        SetAlpha(0f);
    }

    void SetAlpha(float value)
    {
        Color color = vignette.color;
        color.a = value;
        vignette.color = color;
    }
}
