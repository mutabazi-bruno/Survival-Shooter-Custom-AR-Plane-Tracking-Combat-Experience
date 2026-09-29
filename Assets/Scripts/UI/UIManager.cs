using UnityEngine;

// Decides which screen is on show. Listens for game state changes and swaps panels,
// so the game code never has to know the UI exists.
public class UIManager : MonoBehaviour
{
    [SerializeField] UIPanel mainMenu;
    [SerializeField] UIPanel scanning;
    [SerializeField] UIPanel hud;
    [SerializeField] UIPanel gameOver;
    [SerializeField] UIPanel leaderboard;

    UIPanel[] allPanels;

    void Awake()
    {
        allPanels = new[] { mainMenu, scanning, hud, gameOver, leaderboard };

        // start with everything hidden, the first state change will show the right one
        foreach (UIPanel panel in allPanels)
            panel.Hide(instant: true);
    }

    void OnEnable() => GameEvents.StateChanged += OnStateChanged;
    void OnDisable() => GameEvents.StateChanged -= OnStateChanged;

    public void OpenLeaderboard() => ShowOnly(leaderboard);
    public void CloseLeaderboard() => ShowOnly(mainMenu);

    void OnStateChanged(GameStateId state)
    {
        ShowOnly(state switch
        {
            GameStateId.Menu => mainMenu,
            GameStateId.Placing => scanning,
            GameStateId.Playing => hud,
            _ => gameOver
        });
    }

    void ShowOnly(UIPanel target)
    {
        foreach (UIPanel panel in allPanels)
        {
            if (panel != target && panel.IsVisible)
                panel.Hide();
        }
        target.Show();
    }
}
