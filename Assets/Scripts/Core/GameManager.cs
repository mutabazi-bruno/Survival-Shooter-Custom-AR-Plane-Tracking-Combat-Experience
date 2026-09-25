using System.Collections.Generic;
using UnityEngine;

// Runs the game flow: Menu -> Placing -> Playing -> GameOver.
// Holds the current state, the chosen difficulty and the running session.
// Other systems find out what's happening through GameEvents instead of polling this.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] ArenaPlacer placer;
    [SerializeField] DifficultySettings[] difficulties;
    [SerializeField] int defaultDifficulty = 0;

    const string DifficultyKey = "difficulty";

    GameState currentState;

    public ArenaPlacer Placer => placer;
    public IReadOnlyList<DifficultySettings> Difficulties => difficulties;
    public DifficultySettings Difficulty { get; private set; }
    public int DifficultyIndex { get; private set; }
    public GameSession Session { get; private set; }
    public SessionResult LastResult { get; private set; }
    public GameStateId CurrentState => currentState.Id;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // remember the last difficulty the player picked
        SetDifficulty(PlayerPrefs.GetInt(DifficultyKey, defaultDifficulty));
    }

    void Start()
    {
        ChangeState(new MenuState(this));
    }

    void Update()
    {
        currentState?.Tick(Time.deltaTime);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void ChangeState(GameState next)
    {
        currentState?.Exit();
        currentState = next;

        Debug.Log($"Game state: {next.Id}");
        GameEvents.RaiseStateChanged(next.Id);
        next.Enter();
    }

    // --- called by the UI buttons ---

    [ContextMenu("Start Game")]
    public void StartGame() => ChangeState(new PlacingState(this));

    [ContextMenu("Restart")]
    public void RestartGame() => ChangeState(new PlayingState(this));

    [ContextMenu("Main Menu")]
    public void GoToMainMenu() => ChangeState(new MenuState(this));

    public void SetDifficulty(int index)
    {
        if (difficulties == null || difficulties.Length == 0)
        {
            Debug.LogError("GameManager has no difficulty assets assigned.");
            return;
        }

        DifficultyIndex = Mathf.Clamp(index, 0, difficulties.Length - 1);
        Difficulty = difficulties[DifficultyIndex];
        PlayerPrefs.SetInt(DifficultyKey, DifficultyIndex);
    }

    // --- called by the states ---

    public void StartNewSession()
    {
        Session = new GameSession(Difficulty.RoundDuration);
        GameEvents.RaiseRoundStarted();
    }

    public void EndGame(bool survived)
    {
        LastResult = Session.ToResult(survived, Difficulty.DisplayName);
        Debug.Log($"Round over. Score {LastResult.score}, kills {LastResult.enemiesKilled}, survived {LastResult.survived}");
        ChangeState(new GameOverState(this, LastResult));
    }
}
