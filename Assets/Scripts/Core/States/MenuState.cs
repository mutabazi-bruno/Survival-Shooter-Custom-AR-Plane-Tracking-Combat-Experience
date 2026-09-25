// Start menu is showing. Nothing is running, taps on planes are ignored.
public class MenuState : GameState
{
    public MenuState(GameManager game) : base(game) { }

    public override GameStateId Id => GameStateId.Menu;

    public override void Enter()
    {
        game.Placer.SetPlacementEnabled(false);
    }
}
