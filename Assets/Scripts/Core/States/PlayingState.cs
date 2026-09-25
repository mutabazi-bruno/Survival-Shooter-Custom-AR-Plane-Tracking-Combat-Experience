// The actual round. Runs the timer and ends the game when time is up or the player dies.
public class PlayingState : GameState
{
    public PlayingState(GameManager game) : base(game) { }

    public override GameStateId Id => GameStateId.Playing;

    public override void Enter()
    {
        GameEvents.EnemyKilled += OnEnemyKilled;
        GameEvents.PlayerDied += OnPlayerDied;
        game.StartNewSession();
    }

    public override void Tick(float deltaTime)
    {
        game.Session.Tick(deltaTime);

        if (game.Session.IsTimeUp)
            game.EndGame(survived: true);
    }

    public override void Exit()
    {
        GameEvents.EnemyKilled -= OnEnemyKilled;
        GameEvents.PlayerDied -= OnPlayerDied;
    }

    void OnEnemyKilled(int points) => game.Session.AddKill(points);

    void OnPlayerDied() => game.EndGame(survived: false);
}
