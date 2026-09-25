// Anything a bullet or melee hit can hurt: the player and every enemy.
// Bullets don't care what they hit, they just look for this.
public interface IDamageable
{
    bool IsAlive { get; }
    void TakeDamage(int amount);
}
