using TMPro;
using UnityEngine;
using UnityEngine.UI;

// In-game HUD: segmented armour bar, score and time left.
public class HudPanel : UIPanel
{
    [SerializeField] PlayerHealth player;

    [Header("Health")]
    [Tooltip("Armour blocks from left to right. They empty one by one as the player takes damage.")]
    [SerializeField] Image[] segments;
    [SerializeField] TMP_Text healthText;
    [SerializeField] Color healthyColor = new(0.62f, 0.77f, 0.44f);
    [SerializeField] Color lowHealthColor = new(1f, 0.23f, 0.19f);
    [SerializeField] Color emptySegmentColor = new(1f, 1f, 1f, 0.1f);
    [SerializeField, Range(0f, 1f)] float lowHealthPercent = 0.3f;

    [Header("Score and time")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text timerText;
    [SerializeField] int warningSeconds = 10;
    [SerializeField] Color timerColor = Color.white;
    [SerializeField] Color timerWarningColor = new(1f, 0.23f, 0.19f);

    float shownHealth = 1f;  // what the bar shows, eases toward the real value
    float targetHealth = 1f;
    float scorePunch;
    float timerPulse;

    void OnEnable()
    {
        GameEvents.PlayerHealthChanged += OnHealthChanged;
        GameEvents.ScoreChanged += OnScoreChanged;
        GameEvents.TimeChanged += OnTimeChanged;
    }

    void OnDisable()
    {
        GameEvents.PlayerHealthChanged -= OnHealthChanged;
        GameEvents.ScoreChanged -= OnScoreChanged;
        GameEvents.TimeChanged -= OnTimeChanged;
    }

    protected override void OnShow()
    {
        // start the new round on a full bar, the real numbers arrive through events right after
        shownHealth = targetHealth = 1f;
        scorePunch = timerPulse = 0f;
        OnHealthChanged(player.Max, player.Max);
    }

    protected override void Update()
    {
        base.Update();

        shownHealth = Mathf.MoveTowards(shownHealth, targetHealth, Time.deltaTime * 1.5f);
        DrawSegments(shownHealth);

        scorePunch = Mathf.MoveTowards(scorePunch, 0f, Time.deltaTime * 4f);
        scoreText.transform.localScale = Vector3.one * (1f + scorePunch * 0.3f);

        timerPulse = Mathf.MoveTowards(timerPulse, 0f, Time.deltaTime * 2.5f);
        timerText.transform.localScale = Vector3.one * (1f + timerPulse * 0.25f);
    }

    // each block covers an equal slice of health, the last one fades out partly instead of snapping
    void DrawSegments(float health)
    {
        Color full = targetHealth <= lowHealthPercent ? lowHealthColor : healthyColor;
        float slice = 1f / segments.Length;

        for (int i = 0; i < segments.Length; i++)
        {
            float amount = Mathf.Clamp01((health - i * slice) / slice);
            segments[i].color = Color.Lerp(emptySegmentColor, full, amount);
        }
    }

    void OnHealthChanged(int current, int max)
    {
        if (max <= 0) return;

        targetHealth = (float)current / max;
        healthText.text = current.ToString();
        healthText.color = targetHealth <= lowHealthPercent ? lowHealthColor : healthyColor;
    }

    void OnScoreChanged(int score)
    {
        scoreText.text = score.ToString();
        if (score > 0) scorePunch = 1f;
    }

    void OnTimeChanged(int secondsLeft)
    {
        timerText.text = $"{secondsLeft / 60:0}:{secondsLeft % 60:00}";

        bool warning = secondsLeft <= warningSeconds;
        timerText.color = warning ? timerWarningColor : timerColor;
        if (warning) timerPulse = 1f;
    }
}
