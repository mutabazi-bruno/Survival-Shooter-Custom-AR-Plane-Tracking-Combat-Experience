using UnityEngine;

// Spawns enemies at the arena's spawn points while a round is running,
// using the spawn rate, enemy cap and shooter chance from the difficulty.
// When the round ends every enemy is wiped.
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemyFactory factory;
    [SerializeField] ArenaPlacer placer;
    [SerializeField] PlayerHealth player;
    [SerializeField] float firstSpawnDelay = 1.5f;
    [Tooltip("Don't spawn enemies closer than this to the player.")]
    [SerializeField] float minDistanceFromPlayer = 1f;

    ArenaLayout layout;
    bool spawning;
    float nextSpawnTime;

    void OnEnable()
    {
        GameEvents.RoundStarted += BeginSpawning;
        GameEvents.RoundEnded += StopAndWipe;
    }

    void OnDisable()
    {
        GameEvents.RoundStarted -= BeginSpawning;
        GameEvents.RoundEnded -= StopAndWipe;
    }

    void BeginSpawning()
    {
        layout = placer.Arena.GetComponent<ArenaLayout>();
        spawning = true;
        nextSpawnTime = Time.time + firstSpawnDelay;
    }

    void StopAndWipe(SessionResult _)
    {
        spawning = false;
        factory.DespawnAll();
    }

    void Update()
    {
        if (!spawning || Time.time < nextSpawnTime) return;

        DifficultySettings difficulty = GameManager.Instance.Difficulty;
        nextSpawnTime = Time.time + difficulty.SpawnInterval;

        if (factory.ActiveCount >= difficulty.MaxEnemiesAlive) return;

        EnemyType type = Random.value < difficulty.ShooterChance ? EnemyType.Shooter : EnemyType.Melee;
        Vector3 playerPos = player.transform.position;
        Vector3 spawnPos = layout.PickSpawnPoint(playerPos, minDistanceFromPlayer).position;

        Vector3 facing = playerPos - spawnPos;
        facing.y = 0f;
        Quaternion rotation = facing.sqrMagnitude > 0.001f ? Quaternion.LookRotation(facing) : Quaternion.identity;

        factory.Create(type, spawnPos, rotation, player.transform, player, difficulty);
    }
}
