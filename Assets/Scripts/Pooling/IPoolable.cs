// Anything that lives in an ObjectPool can implement this to reset itself
// when it's handed out and when it comes back.
public interface IPoolable
{
    void OnTakenFromPool();
    void OnReturnedToPool();
}
