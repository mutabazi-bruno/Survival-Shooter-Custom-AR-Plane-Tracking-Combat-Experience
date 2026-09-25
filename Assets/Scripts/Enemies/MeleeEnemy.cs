using UnityEngine;

// Rushes straight at the player and slashes once it's close enough.
// Quick but fragile. Can only hurt the player when right next to them.
public class MeleeEnemy : Enemy
{
    [Header("Melee attack")]
    [SerializeField] float attackRange = 0.55f;
    [SerializeField] float attackCooldown = 1.2f;
    [Tooltip("Time into the slash animation when the blade actually lands.")]
    [SerializeField] float hitDelay = 0.35f;
    [SerializeField] int damage = 15;

    float nextAttackTime;
    float swingTimer = -1f; // counts down while a swing is in progress, -1 when not swinging

    public override EnemyType Type => EnemyType.Melee;

    public override void OnTakenFromPool()
    {
        base.OnTakenFromPool();
        nextAttackTime = 0f;
        swingTimer = -1f;
    }

    protected override void Think(float deltaTime)
    {
        if (swingTimer >= 0f)
        {
            swingTimer -= deltaTime;
            if (swingTimer < 0f) LandHit();
            return;
        }

        if (DistanceToPlayer() > attackRange)
        {
            MoveTowardPlayer(deltaTime);
            return;
        }

        if (Time.time >= nextAttackTime)
            StartSwing();
        else
            PlayAnimation("Idle");
    }

    void StartSwing()
    {
        nextAttackTime = Time.time + attackCooldown;
        swingTimer = hitDelay;
        PlayAnimation("Attack", lockFor: 0.6f, force: true);
    }

    void LandHit()
    {
        // the player may have stepped back during the swing, give them a little slack
        if (DistanceToPlayer() > attackRange * 1.3f) return;

        PlayerHealth.TakeDamage(ScaledDamage(damage));
        GameEvents.RaiseEnemyMeleeHit(transform.position);
    }
}
