using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Start screen: title, difficulty choice, Start and Leaderboard buttons.
public class MainMenuPanel : UIPanel
{
    [SerializeField] UIManager ui;
    [SerializeField] Leaderboard leaderboard;
    [SerializeField] Button startButton;
    [SerializeField] Button leaderboardButton;
    [Tooltip("Same order as the difficulties on the GameManager.")]
    [SerializeField] Button[] difficultyButtons;
    [SerializeField] TMP_Text difficultyInfo;
    [SerializeField] TMP_Text bestScoreText;

    [Header("Difficulty buttons")]
    [SerializeField] Sprite selectedSprite;
    [SerializeField] Sprite normalSprite;
    [SerializeField] Color selectedTextColor = new(0.07f, 0.07f, 0.06f);
    [SerializeField] Color normalTextColor = new(0.92f, 0.89f, 0.82f);

    void Awake()
    {
        startButton.onClick.AddListener(() => GameManager.Instance.StartGame());
        leaderboardButton.onClick.AddListener(ui.OpenLeaderboard);

        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            int index = i; // copy for the lambda
            difficultyButtons[i].onClick.AddListener(() => SelectDifficulty(index));
        }
    }

    protected override void OnShow()
    {
        RefreshDifficulty();

        bool hasScores = leaderboard.Sessions.Count > 0;
        bestScoreText.gameObject.SetActive(hasScores);
        if (hasScores)
            bestScoreText.text = $"BEST RECENT SCORE  <color=#F2B705>{leaderboard.BestScore}</color>";
    }

    void SelectDifficulty(int index)
    {
        GameManager.Instance.SetDifficulty(index);
        RefreshDifficulty();
    }

    void RefreshDifficulty()
    {
        int selected = GameManager.Instance.DifficultyIndex;

        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            bool isSelected = i == selected;
            difficultyButtons[i].image.sprite = isSelected ? selectedSprite : normalSprite;
            difficultyButtons[i].GetComponentInChildren<TMP_Text>().color = isSelected ? selectedTextColor : normalTextColor;
        }

        DifficultySettings d = GameManager.Instance.Difficulty;
        difficultyInfo.text = $"{d.RoundDuration:0}s ROUND  ·  {d.PlayerMaxHealth} HP  ·  UP TO {d.MaxEnemiesAlive} MECHS";
    }
}
