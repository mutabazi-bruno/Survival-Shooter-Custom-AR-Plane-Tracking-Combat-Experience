using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One line in the leaderboard.
public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TMP_Text dateText;
    [SerializeField] TMP_Text modeText;
    [SerializeField] TMP_Text killsText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Color normalColor = new(1f, 1f, 1f, 0.06f);
    [SerializeField] Color bestColor = new(0.95f, 0.72f, 0.02f, 0.28f);

    public void Show(SessionResult session, bool isBest)
    {
        background.color = isBest ? bestColor : normalColor;

        dateText.text = session.date;
        // WON in green if they survived the timer, KIA in red if the mechs got them
        modeText.text = session.survived
            ? $"{session.difficulty.ToUpper()} <color=#9DC46F>WON</color>"
            : $"{session.difficulty.ToUpper()} <color=#FF3B30>KIA</color>";
        killsText.text = session.enemiesKilled.ToString();
        timeText.text = GameOverPanel.FormatTime(session.timeSurvived);
        scoreText.text = session.score.ToString();
    }
}
