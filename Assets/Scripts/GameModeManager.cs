using UnityEngine;

public enum GameMode
{
    PlayerVsEnvironment,
    PlayerVsPlayer,
    Tutorial
}

public class GameModeManager : MonoBehaviour
{
    private static GameModeManager instance;
    public GameMode currentGameMode;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persist between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
    }
}
