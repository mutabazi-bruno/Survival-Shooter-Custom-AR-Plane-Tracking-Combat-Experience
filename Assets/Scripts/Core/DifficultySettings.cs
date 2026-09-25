using UnityEngine;

// All the numbers that change between Easy and Hard live here.
// Make one asset per difficulty (Create > Survival Shooter > Difficulty).
[CreateAssetMenu(fileName = "Difficulty", menuName = "Survival Shooter/Difficulty")]
public class DifficultySettings : ScriptableObject
{
    [SerializeField] string displayName = "Normal";

    [Header("Round")]
    [SerializeField, Min(10f)] float roundDuration = 90f;

    [Header("Player")]
    [SerializeField, Min(1)] int playerMaxHealth = 100;

    [Header("Spawning")]
    [SerializeField, Min(0.3f)] float spawnInterval = 3f;
    [SerializeField, Min(1)] int maxEnemiesAlive = 6;
    [SerializeField, Range(0f, 1f)] float shooterChance = 0.35f;

    [Header("Enemies")]
    [SerializeField, Min(0.1f)] float enemySpeedMultiplier = 1f;
    [SerializeField, Min(0.1f)] float enemyDamageMultiplier = 1f;

    public string DisplayName => displayName;
    public float RoundDuration => roundDuration;
    public int PlayerMaxHealth => playerMaxHealth;
    public float SpawnInterval => spawnInterval;
    public int MaxEnemiesAlive => maxEnemiesAlive;
    public float ShooterChance => shooterChance;
    public float EnemySpeedMultiplier => enemySpeedMultiplier;
    public float EnemyDamageMultiplier => enemyDamageMultiplier;
}
