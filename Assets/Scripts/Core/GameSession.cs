using System;
using UnityEngine;

// Keeps track of the numbers for the round being played: score, kills and time.
// A new one is made every time a round starts, so restarting is just "new GameSession".
public class GameSession
{
    public int Score { get; private set; }
    public int EnemiesKilled { get; private set; }
    public float TimeSurvived { get; private set; }
    public float TimeLeft => Mathf.Max(0f, duration - TimeSurvived);
    public bool IsTimeUp => TimeSurvived >= duration;

    readonly float duration;
    int lastSecondShown = -1;

    public GameSession(float duration)
    {
        this.duration = duration;
        GameEvents.RaiseScoreChanged(0);
        RaiseTimeIfChanged();
    }

    public void Tick(float deltaTime)
    {
        TimeSurvived = Mathf.Min(TimeSurvived + deltaTime, duration);
        RaiseTimeIfChanged();
    }

    public void AddKill(int points)
    {
        EnemiesKilled++;
        Score += points;
        GameEvents.RaiseScoreChanged(Score);
    }

    public SessionResult ToResult(bool survived, string difficulty)
    {
        return new SessionResult
        {
            score = Score,
            enemiesKilled = EnemiesKilled,
            timeSurvived = TimeSurvived,
            survived = survived,
            difficulty = difficulty,
            date = DateTime.Now.ToString("dd MMM, HH:mm")
        };
    }

    // the HUD only shows whole seconds, so no need to fire this every frame
    void RaiseTimeIfChanged()
    {
        int seconds = Mathf.CeilToInt(TimeLeft);
        if (seconds == lastSecondShown) return;

        lastSecondShown = seconds;
        GameEvents.RaiseTimeChanged(seconds);
    }
}
