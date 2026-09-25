using UnityEngine;

// A pooled bullet. Moves itself each frame and sphere-casts ahead so fast bullets
// can't skip through thin enemies. Goes back to its pool on hit or when it times out.
public class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField] float speed = 8f;
    [SerializeField] float lifetime = 2f;
    [SerializeField] float radius = 0.03f;
    [Tooltip("Layers this bullet can hit. Player bullets: Default + Enemy. Enemy bullets: Default + Player.")]
    [SerializeField] LayerMask hitMask = ~0;
    [SerializeField] TrailRenderer trail;

    ProjectilePool pool;
    Vector3 direction;
    int damage;
    float timeLeft;

    public void SetPool(ProjectilePool owner) => pool = owner;

    public void Launch(Vector3 position, Vector3 moveDirection, int hitDamage)
    {
        transform.SetPositionAndRotation(position, Quaternion.LookRotation(moveDirection));
        direction = moveDirection;
        damage = hitDamage;

        // otherwise the trail draws a line from where this bullet was last time
        if (trail != null) trail.Clear();
    }

    public void OnTakenFromPool()
    {
        timeLeft = lifetime;
    }

    public void OnReturnedToPool()
    {
        damage = 0;
        direction = Vector3.zero;
        if (trail != null) trail.Clear();
    }

    void Update()
    {
        float step = speed * Time.deltaTime;

        if (Physics.SphereCast(transform.position, radius, direction, out RaycastHit hit, step, hitMask, QueryTriggerInteraction.Collide))
        {
            Hit(hit.collider);
            return;
        }

        transform.position += direction * step;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
            pool.Return(this);
    }

    void Hit(Collider other)
    {
        // colliders are often on child meshes, the health script sits on the root
        var target = other.GetComponentInParent<IDamageable>();
        if (target != null && target.IsAlive)
            target.TakeDamage(damage);

        pool.Return(this);
    }
}
