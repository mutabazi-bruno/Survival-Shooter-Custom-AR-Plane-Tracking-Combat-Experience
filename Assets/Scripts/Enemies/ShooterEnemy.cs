using UnityEngine;

// Walks in until it's at shooting distance, then stands still and fires at the player.
// Slower and tougher than the melee enemy, and it can hurt you from much further away.
public class ShooterEnemy : Enemy
{
    [Header("Ranged attack")]
    [SerializeField] float shootingDistance = 1.6f;
    [SerializeField] float fireCooldown = 2f;
    [Tooltip("Time into the shoot animation when the bullet leaves the gun.")]
    [SerializeField] float fireDelay = 0.25f;
    [SerializeField] int damage = 8;
    [SerializeField] Transform muzzle;

    ProjectilePool bulletPool;
    float nextShotTime;
    float aimTimer = -1f; // counts down while aiming, -1 when not

    public override EnemyType Type => EnemyType.Shooter;

    // the factory hands us the shared enemy bullet pool once, when the enemy is created
    public void SetBulletPool(ProjectilePool pool) => bulletPool = pool;

    public override void OnTakenFromPool()
    {
        base.OnTakenFromPool();
        // small random delay so a group of shooters doesn't fire in perfect sync
        nextShotTime = Time.time + Random.Range(0.6f, 1.2f);
        aimTimer = -1f;
    }

    protected override void Think(float deltaTime)
    {
        if (aimTimer >= 0f)
        {
            aimTimer -= deltaTime;
            if (aimTimer < 0f) FireBullet();
            return;
        }

        if (DistanceToPlayer() > shootingDistance)
        {
            MoveTowardPlayer(deltaTime);
            return;
        }

        if (Time.time >= nextShotTime)
            StartShot();
        else
            PlayAnimation("Idle");
    }

    void StartShot()
    {
        nextShotTime = Time.time + fireCooldown;
        aimTimer = fireDelay;
        PlayAnimation("Attack", lockFor: 0.5f, force: true);
    }

    void FireBullet()
    {
        Vector3 from = muzzle != null ? muzzle.position : transform.position + Vector3.up * 0.3f;
        Vector3 direction = (Player.position - from).normalized;

        bulletPool.Fire(from, direction, ScaledDamage(damage));
        GameEvents.RaiseEnemyFired(from);
    }
}
