using System;

// Summary of one finished round. Used by the end screen and saved to the leaderboard.
[Serializable]
public struct SessionResult
{
    public int score;
    public int enemiesKilled;
    public float timeSurvived;
    public bool survived;
    public string difficulty;
    public string date;
}
