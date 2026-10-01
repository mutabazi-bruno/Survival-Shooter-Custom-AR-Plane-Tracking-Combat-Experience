using System;
using UnityEngine;

// A particle effect that lives in a pool. Plays from the start every time it's taken out,
// then hands itself back once the particles have finished.
// Playing waits until the first Update so whoever took it out has had a chance to move it into place.
public class PooledEffect : MonoBehaviour, IPoolable
{
    [SerializeField] float lifetime = 1.5f;

    ParticleSystem[] systems;
    Action<PooledEffect> returnToPool;
    float timeLeft;
    bool waitingToPlay;

    void Awake()
    {
        systems = GetComponentsInChildren<ParticleSystem>(true);
    }

    public void SetPool(Action<PooledEffect> onFinished) => returnToPool = onFinished;

    public void OnTakenFromPool()
    {
        timeLeft = lifetime;
        waitingToPlay = true;
    }

    public void OnReturnedToPool()
    {
        foreach (ParticleSystem ps in systems)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Update()
    {
        if (waitingToPlay)
        {
            waitingToPlay = false;
            foreach (ParticleSystem ps in systems)
            {
                ps.Clear(true);
                ps.Play(true);
            }
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f) returnToPool?.Invoke(this);
    }
}
