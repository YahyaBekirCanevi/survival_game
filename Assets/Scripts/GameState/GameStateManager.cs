public class GameStateManager
{
    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            _instance ??= new GameStateManager();
            return _instance;
        }
    }
    private GameStateManager() { }
    public void SetState(GameState newGameState)
    {
        if (newGameState == CurrentGameState) return;
        CurrentGameState = newGameState;
        OnGameStateChanged?.Invoke(newGameState);
    }
    public GameState CurrentGameState { get; private set; }
    public delegate void GameStateChangeHandler(GameState newGameState);
    public event GameStateChangeHandler OnGameStateChanged;
}
