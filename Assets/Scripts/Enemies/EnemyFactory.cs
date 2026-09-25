using System.Collections.Generic;
using UnityEngine;

// Factory pattern: the spawner just asks for "a shooter here" and gets back a ready enemy.
// Behind that, each enemy type has its own pre-filled pool, so enemies are never
// Instantiated or Destroyed during a round either.
public class EnemyFactory : MonoBehaviour
{
    [SerializeField] MeleeEnemy meleePrefab;
    [SerializeField] ShooterEnemy shooterPrefab;
    [Tooltip("Should be at least the highest Max Enemies Alive of any difficulty.")]
    [SerializeField, Min(1)] int poolSizePerType = 8;
    [SerializeField] ProjectilePool enemyBulletPool;

    readonly Dictionary<EnemyType, ObjectPool<Enemy>> pools = new();

    public int ActiveCount
    {
        get
        {
            int count = 0;
            foreach (ObjectPool<Enemy> pool in pools.Values)
                count += pool.CountInUse;
            return count;
        }
    }

    void Awake()
    {
        pools[EnemyType.Melee] = new ObjectPool<Enemy>(meleePrefab, poolSizePerType, transform);
        pools[EnemyType.Shooter] = new ObjectPool<Enemy>(shooterPrefab, poolSizePerType, transform,
            enemy => ((ShooterEnemy)enemy).SetBulletPool(enemyBulletPool));
    }

    public Enemy Create(EnemyType type, Vector3 position, Quaternion rotation,
                        Transform player, IDamageable playerHealth, DifficultySettings difficulty)
    {
        Enemy enemy = pools[type].Get();
        enemy.transform.SetPositionAndRotation(position, rotation);
        enemy.Init(player, playerHealth, difficulty, Release);
        return enemy;
    }

    public void DespawnAll()
    {
        foreach (ObjectPool<Enemy> pool in pools.Values)
            pool.ReleaseAll();
    }

    void Release(Enemy enemy) => pools[enemy.Type].Release(enemy);
}
