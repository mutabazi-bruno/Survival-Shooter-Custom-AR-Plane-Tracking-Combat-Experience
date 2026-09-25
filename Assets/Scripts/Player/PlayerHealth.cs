using UnityEngine;

// The player's health. Lives on the AR camera since in FPS the phone *is* the player.
// Max health comes from the chosen difficulty and is refilled at the start of every round.
public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int Current { get; private set; }
    public int Max { get; private set; }
    public bool IsAlive => Current > 0;

    void OnEnable() => GameEvents.RoundStarted += Refill;
    void OnDisable() => GameEvents.RoundStarted -= Refill;

    void Refill()
    {
        Max = GameManager.Instance.Difficulty.PlayerMaxHealth;
        Current = Max;
        GameEvents.RaisePlayerHealthChanged(Current, Max);
    }

    public void TakeDamage(int amount)
    {
        // ignore hits that land after the round already ended
        if (!IsAlive || GameManager.Instance.CurrentState != GameStateId.Playing) return;

        Current = Mathf.Max(0, Current - amount);
        GameEvents.RaisePlayerHealthChanged(Current, Max);
        GameEvents.RaisePlayerDamaged();

        if (Current == 0)
            GameEvents.RaisePlayerDied();
    }

    // handy for testing damage before enemies exist
    [ContextMenu("Take 20 damage")]
    void TestHit() => TakeDamage(20);
}
