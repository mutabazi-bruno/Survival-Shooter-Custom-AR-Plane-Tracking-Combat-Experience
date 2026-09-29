using System;
using UnityEngine;
using Random = UnityEngine.Random;

// One sound effect with a few variations. Picking a random clip and nudging the pitch
// stops rapid sounds (like shooting) from sounding like the same sample on repeat.
[Serializable]
public class Sound
{
    [SerializeField] AudioClip[] clips;
    [SerializeField, Range(0f, 1f)] float volume = 1f;
    [SerializeField, Range(0f, 0.3f)] float pitchVariation = 0.05f;

    public float Volume => volume;
    public bool HasClips => clips != null && clips.Length > 0;

    public AudioClip PickClip() => HasClips ? clips[Random.Range(0, clips.Length)] : null;

    public float PickPitch() => 1f + Random.Range(-pitchVariation, pitchVariation);
}
