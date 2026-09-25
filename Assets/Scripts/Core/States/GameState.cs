public enum GameStateId { Menu, Placing, Playing, GameOver }

// Base class for the game states (State pattern).
// GameManager only knows it has "a state" and calls Enter / Tick / Exit on it,
// each state decides for itself what happens and when to move on.
public abstract class GameState
{
    protected readonly GameManager game;

    protected GameState(GameManager game)
    {
        this.game = game;
    }

    public abstract GameStateId Id { get; }

    public virtual void Enter() { }
    public virtual void Tick(float deltaTime) { }
    public virtual void Exit() { }
}
