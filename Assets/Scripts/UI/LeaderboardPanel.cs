using UnityEngine;
using UnityEngine.UI;

// Shows the last 5 rounds, newest at the top, with the best one highlighted.
public class LeaderboardPanel : UIPanel
{
    [SerializeField] UIManager ui;
    [SerializeField] Leaderboard leaderboard;
    [SerializeField] LeaderboardRow[] rows;
    [SerializeField] GameObject emptyMessage;
    [SerializeField] Button backButton;

    void Awake()
    {
        backButton.onClick.AddListener(ui.CloseLeaderboard);
    }

    protected override void OnShow()
    {
        var sessions = leaderboard.Sessions;
        int best = leaderboard.BestScore;

        for (int i = 0; i < rows.Length; i++)
        {
            bool hasEntry = i < sessions.Count;
            rows[i].gameObject.SetActive(hasEntry);

            if (hasEntry)
                rows[i].Show(sessions[i], isBest: sessions[i].score == best && best > 0);
        }

        emptyMessage.SetActive(sessions.Count == 0);
    }
}
