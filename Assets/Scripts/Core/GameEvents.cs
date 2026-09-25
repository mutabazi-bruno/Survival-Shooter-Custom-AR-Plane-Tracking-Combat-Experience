using System;
using UnityEngine;

// One place for game-wide events (Observer pattern).
// Gameplay code raises them, and the UI / audio / spawner just listen,
// so none of those systems need a direct reference to each other.
public static class GameEvents
{
    public static event Action<GameStateId> StateChanged;
    public static event Action RoundStarted;
    public static event Action<SessionResult> RoundEnded;

    public static event Action<int> ScoreChanged;            // new total score
    public static event Action<int> TimeChanged;             // whole seconds left
    public static event Action<int, int> PlayerHealthChanged; // current, max
    public static event Action PlayerFired;
    public static event Action PlayerDamaged;
    public static event Action PlayerDied;
    public static event Action<int> EnemyKilled;             // points that enemy was worth
    public static event Action<Vector3> EnemySpawned;
    public static event Action<Vector3> EnemyHit;
    public static event Action<Vector3> EnemyFired;
    public static event Action<Vector3> EnemyMeleeHit;

    public static void RaiseStateChanged(GameStateId state) => StateChanged?.Invoke(state);
    public static void RaiseRoundStarted() => RoundStarted?.Invoke();
    public static void RaiseRoundEnded(SessionResult result) => RoundEnded?.Invoke(result);

    public static void RaiseScoreChanged(int score) => ScoreChanged?.Invoke(score);
    public static void RaiseTimeChanged(int secondsLeft) => TimeChanged?.Invoke(secondsLeft);
    public static void RaisePlayerHealthChanged(int current, int max) => PlayerHealthChanged?.Invoke(current, max);
    public static void RaisePlayerFired() => PlayerFired?.Invoke();
    public static void RaisePlayerDamaged() => PlayerDamaged?.Invoke();
    public static void RaisePlayerDied() => PlayerDied?.Invoke();
    public static void RaiseEnemyKilled(int points) => EnemyKilled?.Invoke(points);
    public static void RaiseEnemySpawned(Vector3 position) => EnemySpawned?.Invoke(position);
    public static void RaiseEnemyHit(Vector3 position) => EnemyHit?.Invoke(position);
    public static void RaiseEnemyFired(Vector3 position) => EnemyFired?.Invoke(position);
    public static void RaiseEnemyMeleeHit(Vector3 position) => EnemyMeleeHit?.Invoke(position);

    // static events survive between play sessions in the editor, so clear them on every start
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ClearListeners()
    {
        StateChanged = null;
        RoundStarted = null;
        RoundEnded = null;
        ScoreChanged = null;
        TimeChanged = null;
        PlayerHealthChanged = null;
        PlayerFired = null;
        PlayerDamaged = null;
        PlayerDied = null;
        EnemyKilled = null;
        EnemySpawned = null;
        EnemyHit = null;
        EnemyFired = null;
        EnemyMeleeHit = null;
    }
}
