using UnityEngine;

// Plays every sound in the game (Singleton + Observer).
// Nothing calls this directly for gameplay sounds, it just listens to GameEvents.
//
// AudioSources used, all created once in Awake:
//  - music:  one looping 2D source, fades between menu and battle music
//  - ui:     one 2D source for the player's own sounds and button clicks
//  - voice:  one 2D source for the announcer, a new line cuts off the old one
//  - effects: a small pool of 3D sources for enemy sounds, placed where the enemy is,
//             so in AR you can hear which side a robot is coming from
//  - steps:  a separate tiny pool for footsteps so they never steal a source from gunfire
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip gameMusic;
    [SerializeField, Range(0f, 1f)] float musicVolume = 0.35f;
    [SerializeField] float musicFadeTime = 1f;

    [Header("Player")]
    [SerializeField] Sound playerShoot;
    [SerializeField] Sound playerDeath;

    [Header("Enemies")]
    [SerializeField] Sound enemySpawn;
    [SerializeField] Sound enemyShoot;
    [SerializeField] Sound meleeHit;
    [SerializeField] Sound enemyHit;
    [SerializeField] Sound enemyDeath;
    [SerializeField] Sound mechStep;

    [Header("Announcer")]
    [SerializeField] AudioClip prepareYourself;
    [SerializeField] AudioClip fight;
    [SerializeField] AudioClip youWin;
    [SerializeField] AudioClip youLose;
    [Tooltip("Index 0 = \"1\", index 4 = \"5\". Played for the last seconds of a round.")]
    [SerializeField] AudioClip[] countdown;
    [SerializeField, Range(0f, 1f)] float voiceVolume = 0.9f;

    [Header("UI")]
    [SerializeField] Sound click;
    [SerializeField] Sound hover;

    [Header("3D sources")]
    [SerializeField, Min(1)] int effectSources = 10;
    [SerializeField, Min(1)] int stepSources = 4;
    [SerializeField] float minDistance = 0.4f;
    [SerializeField] float maxDistance = 6f;

    AudioSource musicSource;
    AudioSource uiSource;
    AudioSource voiceSource;
    AudioSource[] effectPool;
    AudioSource[] stepPool;
    int nextEffect;
    int nextStep;

    AudioClip targetMusic;
    bool playingRound;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        musicSource = Make2DSource("Music");
        musicSource.loop = true;
        uiSource = Make2DSource("UI");
        voiceSource = Make2DSource("Voice");

        effectPool = Make3DSources("Effect", effectSources);
        stepPool = Make3DSources("Step", stepSources);
    }

    void OnEnable()
    {
        GameEvents.StateChanged += OnStateChanged;
        GameEvents.RoundStarted += OnRoundStarted;
        GameEvents.RoundEnded += OnRoundEnded;
        GameEvents.TimeChanged += OnTimeChanged;
        GameEvents.PlayerFired += OnPlayerFired;
        GameEvents.PlayerDied += OnPlayerDied;
        GameEvents.EnemySpawned += OnEnemySpawned;
        GameEvents.EnemyFired += OnEnemyFired;
        GameEvents.EnemyMeleeHit += OnMeleeHit;
        GameEvents.EnemyHit += OnEnemyHit;
        GameEvents.EnemyStep += OnEnemyStep;
        GameEvents.EnemyDied += OnEnemyDied;
    }

    void OnDisable()
    {
        GameEvents.StateChanged -= OnStateChanged;
        GameEvents.RoundStarted -= OnRoundStarted;
        GameEvents.RoundEnded -= OnRoundEnded;
        GameEvents.TimeChanged -= OnTimeChanged;
        GameEvents.PlayerFired -= OnPlayerFired;
        GameEvents.PlayerDied -= OnPlayerDied;
        GameEvents.EnemySpawned -= OnEnemySpawned;
        GameEvents.EnemyFired -= OnEnemyFired;
        GameEvents.EnemyMeleeHit -= OnMeleeHit;
        GameEvents.EnemyHit -= OnEnemyHit;
        GameEvents.EnemyStep -= OnEnemyStep;
        GameEvents.EnemyDied -= OnEnemyDied;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        UpdateMusicFade();
    }

    // --- public, for UI buttons ---

    public void PlayClick() => Play2D(click);
    public void PlayHover() => Play2D(hover);

    // --- event handlers ---

    void OnStateChanged(GameStateId state)
    {
        bool inBattle = state == GameStateId.Placing || state == GameStateId.Playing;
        FadeMusicTo(inBattle ? gameMusic : menuMusic);

        if (state != GameStateId.Playing) playingRound = false;
        if (state == GameStateId.Placing) Say(prepareYourself);
    }

    void OnRoundStarted()
    {
        playingRound = true;
        Say(fight);
    }

    void OnRoundEnded(SessionResult result)
    {
        playingRound = false;
        Say(result.survived ? youWin : youLose);
    }

    void OnTimeChanged(int secondsLeft)
    {
        if (!playingRound || countdown == null) return;
        if (secondsLeft >= 1 && secondsLeft <= countdown.Length)
            Say(countdown[secondsLeft - 1]);
    }

    void OnPlayerFired() => Play2D(playerShoot);
    void OnPlayerDied() => Play2D(playerDeath);

    void OnEnemySpawned(Vector3 position) => PlayAt(enemySpawn, position, effectPool, ref nextEffect);
    void OnEnemyFired(Vector3 position) => PlayAt(enemyShoot, position, effectPool, ref nextEffect);
    void OnMeleeHit(Vector3 position) => PlayAt(meleeHit, position, effectPool, ref nextEffect);
    void OnEnemyHit(Vector3 position) => PlayAt(enemyHit, position, effectPool, ref nextEffect);
    void OnEnemyStep(Vector3 position) => PlayAt(mechStep, position, stepPool, ref nextStep);

    void OnEnemyDied(Vector3 position) => PlayAt(enemyDeath, position, effectPool, ref nextEffect);

    // --- playing ---

    void Play2D(Sound sound)
    {
        if (sound == null || !sound.HasClips) return;
        uiSource.pitch = sound.PickPitch();
        uiSource.PlayOneShot(sound.PickClip(), sound.Volume);
    }

    // takes the next source in the pool round-robin, so the oldest sound gets cut if we run out
    void PlayAt(Sound sound, Vector3 position, AudioSource[] pool, ref int next)
    {
        if (sound == null || !sound.HasClips) return;

        AudioSource source = pool[next];
        next = (next + 1) % pool.Length;

        source.transform.position = position;
        source.clip = sound.PickClip();
        source.volume = sound.Volume;
        source.pitch = sound.PickPitch();
        source.Play();
    }

    void Say(AudioClip line)
    {
        if (line == null) return;
        voiceSource.clip = line;
        voiceSource.volume = voiceVolume;
        voiceSource.Play();
    }

    // --- music ---

    void FadeMusicTo(AudioClip clip)
    {
        if (clip == null || clip == targetMusic) return;
        targetMusic = clip;
    }

    // fade the current track out, swap, then fade the new one in
    void UpdateMusicFade()
    {
        if (targetMusic == null) return;

        float step = musicVolume / Mathf.Max(0.01f, musicFadeTime) * Time.unscaledDeltaTime;

        if (musicSource.clip != targetMusic)
        {
            musicSource.volume = Mathf.MoveTowards(musicSource.volume, 0f, step);
            if (musicSource.volume > 0f && musicSource.isPlaying) return;

            musicSource.clip = targetMusic;
            musicSource.Play();
        }

        musicSource.volume = Mathf.MoveTowards(musicSource.volume, musicVolume, step);
    }

    // --- setup ---

    AudioSource Make2DSource(string sourceName)
    {
        var go = new GameObject(sourceName);
        go.transform.SetParent(transform, false);

        var source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        return source;
    }

    AudioSource[] Make3DSources(string sourceName, int count)
    {
        var sources = new AudioSource[count];
        for (int i = 0; i < count; i++)
        {
            AudioSource source = Make2DSource($"{sourceName} {i}");
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.dopplerLevel = 0f;
            sources[i] = source;
        }
        return sources;
    }
}
