using System;
using UnityEngine;

public enum EnemyType { Melee, Shooter }

// Base class for every enemy. Handles everything enemies have in common:
// health, walking toward the player, hit flash, dying and giving score.
// Each enemy type only decides what it does each frame by overriding Think().
[RequireComponent(typeof(Collider))]
public abstract class Enemy : MonoBehaviour, IDamageable, IPoolable
{
    [Header("Stats")]
    [Tooltip("How many player bullets it takes to kill this enemy.")]
    [SerializeField, Min(1)] int maxHealth = 3;
    [SerializeField, Min(0f)] float moveSpeed = 0.4f;
    [SerializeField] float turnSpeed = 360f;
    [SerializeField] int scoreValue = 10;

    [Header("Feedback")]
    [SerializeField] Animator animator;
    [SerializeField] Color hitColor = new(1f, 0.3f, 0.3f);
    [SerializeField] float hitFlashTime = 0.12f;
    [SerializeField] float spawnGrowTime = 0.35f;
    [Tooltip("Gives the death animation time to play before going back to the pool.")]
    [SerializeField] float despawnDelay = 1.5f;

    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    Renderer[] renderers;
    MaterialPropertyBlock block;
    Collider hitbox;
    Vector3 fullScale;
    Action<Enemy> returnToPool;

    int health;
    float flashTimer;
    float growTimer;
    float despawnTimer;
    float animLockTimer;
    string currentAnim;

    protected Transform Player { get; private set; }
    protected IDamageable PlayerHealth { get; private set; }
    protected float SpeedMultiplier { get; private set; } = 1f;
    protected float DamageMultiplier { get; private set; } = 1f;

    public abstract EnemyType Type { get; }
    public bool IsAlive => health > 0;

    protected virtual void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        block = new MaterialPropertyBlock();
        hitbox = GetComponent<Collider>();
        fullScale = transform.localScale;
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    // called by the factory every time this enemy is pulled out of the pool
    public void Init(Transform player, IDamageable playerHealth, DifficultySettings difficulty, Action<Enemy> onFinished)
    {
        Player = player;
        PlayerHealth = playerHealth;
        SpeedMultiplier = difficulty.EnemySpeedMultiplier;
        DamageMultiplier = difficulty.EnemyDamageMultiplier;
        returnToPool = onFinished;

        GameEvents.RaiseEnemySpawned(transform.position);
    }

    public virtual void OnTakenFromPool()
    {
        health = maxHealth;
        hitbox.enabled = true;
        flashTimer = 0f;
        despawnTimer = 0f;
        animLockTimer = 0f;
        currentAnim = null;

        // pop in from nothing instead of just appearing
        growTimer = 0f;
        transform.localScale = Vector3.zero;
    }

    public virtual void OnReturnedToPool()
    {
        Player = null;
        PlayerHealth = null;
        returnToPool = null;
        ClearFlash();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        UpdateSpawnGrow(dt);
        UpdateFlash(dt);
        if (animLockTimer > 0f) animLockTimer -= dt;

        if (!IsAlive)
        {
            despawnTimer -= dt;
            if (despawnTimer <= 0f) Despawn();
            return;
        }

        if (Player == null) return;

        FacePlayer(dt);
        Think(dt);
    }

    // what this enemy does every frame while it's alive (move, attack, wait...)
    protected abstract void Think(float deltaTime);

    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;

        health -= amount;
        flashTimer = hitFlashTime;
        SetFlash();
        GameEvents.RaiseEnemyHit(transform.position);

        if (health <= 0)
            Die();
        else
            PlayAnimation("Hit", lockFor: 0.25f);
    }

    public void Despawn() => returnToPool?.Invoke(this);

    void Die()
    {
        health = 0;
        hitbox.enabled = false;
        despawnTimer = despawnDelay;
        PlayAnimation("Death", force: true);
        GameEvents.RaiseEnemyKilled(scoreValue);
    }

    // --- helpers for the enemy types ---

    // distance along the floor, the camera is higher up than the enemy so height is ignored
    protected float DistanceToPlayer()
    {
        Vector3 offset = Player.position - transform.position;
        offset.y = 0f;
        return offset.magnitude;
    }

    protected void MoveTowardPlayer(float deltaTime)
    {
        Vector3 direction = Player.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;

        transform.position += direction.normalized * (moveSpeed * SpeedMultiplier * deltaTime);
        PlayAnimation("Run");
    }

    protected int ScaledDamage(int baseDamage)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseDamage * DamageMultiplier));
    }

    // the animator just needs states called Idle, Run, Attack, Hit and Death
    protected void PlayAnimation(string state, float lockFor = 0f, bool force = false)
    {
        if (animator == null) return;
        if (!force && (animLockTimer > 0f || state == currentAnim)) return;

        animator.CrossFadeInFixedTime(state, 0.1f);
        currentAnim = state;
        animLockTimer = lockFor;
    }

    void FacePlayer(float deltaTime)
    {
        Vector3 direction = Player.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion look = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * deltaTime);
    }

    void UpdateSpawnGrow(float deltaTime)
    {
        if (growTimer >= spawnGrowTime) return;

        growTimer += deltaTime;
        float t = Mathf.Clamp01(growTimer / spawnGrowTime);
        transform.localScale = fullScale * Mathf.SmoothStep(0f, 1f, t);
    }

    void UpdateFlash(float deltaTime)
    {
        if (flashTimer <= 0f) return;

        flashTimer -= deltaTime;
        if (flashTimer <= 0f) ClearFlash();
    }

    void SetFlash()
    {
        foreach (Renderer r in renderers)
        {
            r.GetPropertyBlock(block);
            block.SetColor(BaseColorId, hitColor);
            r.SetPropertyBlock(block);
        }
    }

    // an empty block puts the renderer back to its normal material colour
    void ClearFlash()
    {
        foreach (Renderer r in renderers)
            r.SetPropertyBlock(null);
    }
}
