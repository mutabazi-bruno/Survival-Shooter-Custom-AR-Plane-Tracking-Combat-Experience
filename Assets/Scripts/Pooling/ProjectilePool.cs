using UnityEngine;

// Scene component that owns one pool of bullets. We use two of these:
// one for the player's bullets and one for the shooter enemies' bullets.
public class ProjectilePool : MonoBehaviour
{
    [SerializeField] Projectile prefab;
    [SerializeField, Min(1)] int poolSize = 30;

    ObjectPool<Projectile> pool;

    public int ActiveCount => pool.CountInUse;

    void Awake()
    {
        pool = new ObjectPool<Projectile>(prefab, poolSize, transform, bullet => bullet.SetPool(this));
    }

    void OnEnable() => GameEvents.RoundEnded += ClearBullets;
    void OnDisable() => GameEvents.RoundEnded -= ClearBullets;

    public Projectile Fire(Vector3 position, Vector3 direction, int damage)
    {
        Projectile bullet = pool.Get();
        bullet.Launch(position, direction, damage);
        return bullet;
    }

    public void Return(Projectile bullet) => pool.Release(bullet);

    // nothing should still be flying on the game over screen
    void ClearBullets(SessionResult _) => pool.ReleaseAll();
}
