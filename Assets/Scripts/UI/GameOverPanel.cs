using TMPro;
using UnityEngine;
using UnityEngine.UI;

// End of round summary: result, final score, enemies defeated, time survived.
public class GameOverPanel : UIPanel
{
    [SerializeField] Leaderboard leaderboard;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text subtitleText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text killsText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] GameObject newBestBadge;
    [SerializeField] Button restartButton;
    [SerializeField] Button menuButton;
    [SerializeField] Color survivedColor = new(0f, 0.9f, 0.78f);
    [SerializeField] Color diedColor = new(1f, 0.23f, 0.19f);
    [SerializeField] float countUpTime = 0.8f;

    int finalScore;
    float countTimer;

    void Awake()
    {
        restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
        menuButton.onClick.AddListener(() => GameManager.Instance.GoToMainMenu());
    }

    // This runs as soon as the state switches to GameOver, which is just before
    // the leaderboard saves the round, so IsNewBest is still comparing against older scores.
    protected override void OnShow()
    {
        SessionResult result = GameManager.Instance.LastResult;

        titleText.text = result.survived ? "YOU SURVIVED" : "YOU DIED";
        titleText.color = result.survived ? survivedColor : diedColor;
        subtitleText.text = result.survived ? "THE ARENA IS YOURS" : "THE MECHS TOOK YOU DOWN";

        killsText.text = result.enemiesKilled.ToString();
        timeText.text = FormatTime(result.timeSurvived);
        newBestBadge.SetActive(leaderboard.IsNewBest(result.score));

        finalScore = result.score;
        countTimer = 0f;
        scoreText.text = "0";
    }

    protected override void Update()
    {
        base.Update();

        // score counts up instead of just appearing
        if (countTimer >= countUpTime) return;
        countTimer += Time.deltaTime;
        int shown = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, countTimer / countUpTime));
        scoreText.text = shown.ToString();
    }

    public static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        return $"{total / 60}:{total % 60:00}";
    }
}
