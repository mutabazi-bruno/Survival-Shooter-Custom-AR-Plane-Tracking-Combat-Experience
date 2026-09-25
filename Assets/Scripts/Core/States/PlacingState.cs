using UnityEngine;

// Player is scanning the room and has to tap a plane to drop the arena.
public class PlacingState : GameState
{
    public PlacingState(GameManager game) : base(game) { }

    public override GameStateId Id => GameStateId.Placing;

    public override void Enter()
    {
        // coming back from the main menu, the arena is already on the floor
        if (game.Placer.IsPlaced)
        {
            game.ChangeState(new PlayingState(game));
            return;
        }

        game.Placer.ArenaPlaced += OnArenaPlaced;
        game.Placer.SetPlacementEnabled(true);
    }

    public override void Exit()
    {
        game.Placer.ArenaPlaced -= OnArenaPlaced;
        game.Placer.SetPlacementEnabled(false);
    }

    void OnArenaPlaced(Transform arena)
    {
        game.ChangeState(new PlayingState(game));
    }
}
