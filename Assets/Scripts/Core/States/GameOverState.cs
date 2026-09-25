// Round is over. Tells everyone the result so the end screen, leaderboard
// and spawner (to wipe enemies) can react. Waits for Restart or Main Menu.
public class GameOverState : GameState
{
    readonly SessionResult result;

    public GameOverState(GameManager game, SessionResult result) : base(game)
    {
        this.result = result;
    }

    public override GameStateId Id => GameStateId.GameOver;

    public override void Enter()
    {
        GameEvents.RaiseRoundEnded(result);
    }
}
