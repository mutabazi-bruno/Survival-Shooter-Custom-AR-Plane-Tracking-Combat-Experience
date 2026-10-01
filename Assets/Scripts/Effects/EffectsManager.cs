using UnityEngine;

// Spawns the combat effects: sparks where bullets land and an explosion when a mech dies.
// Same idea as the bullets: everything is pooled up front, and it only listens to GameEvents.
public class EffectsManager : MonoBehaviour
{
    [SerializeField] PooledEffect sparksPrefab;
    [SerializeField] PooledEffect explosionPrefab;
    [SerializeField, Min(1)] int sparksPoolSize = 16;
    [SerializeField, Min(1)] int explosionPoolSize = 6;

    ObjectPool<PooledEffect> sparks;
    ObjectPool<PooledEffect> explosions;

    void Awake()
    {
        sparks = MakePool(sparksPrefab, sparksPoolSize);
        explosions = MakePool(explosionPrefab, explosionPoolSize);
    }

    void OnEnable()
    {
        GameEvents.ProjectileImpact += OnImpact;
        GameEvents.EnemyDied += OnEnemyDied;
        GameEvents.RoundEnded += ClearAll;
    }

    void OnDisable()
    {
        GameEvents.ProjectileImpact -= OnImpact;
        GameEvents.EnemyDied -= OnEnemyDied;
        GameEvents.RoundEnded -= ClearAll;
    }

    void OnImpact(Vector3 position) => Spawn(sparks, position);
    void OnEnemyDied(Vector3 position) => Spawn(explosions, position);

    void ClearAll(SessionResult _)
    {
        sparks.ReleaseAll();
        explosions.ReleaseAll();
    }

    static void Spawn(ObjectPool<PooledEffect> pool, Vector3 position)
    {
        PooledEffect effect = pool.Get();
        effect.transform.position = position;
    }

    ObjectPool<PooledEffect> MakePool(PooledEffect prefab, int size)
    {
        ObjectPool<PooledEffect> pool = null;
        pool = new ObjectPool<PooledEffect>(prefab, size, transform, effect => effect.SetPool(e => pool.Release(e)));
        return pool;
    }
}
