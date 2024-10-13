using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Button[] levelButtons; // Array to hold all level buttons

    void Start()
    {
        // Assuming the first button is for Player vs AI
        levelButtons[0].onClick.AddListener(() => LoadBattlefieldScene(GameMode.PlayerVsEnvironment));
        // Assuming the second button is for Player vs Player
        levelButtons[1].onClick.AddListener(() => LoadBattlefieldScene(GameMode.PlayerVsPlayer));
        levelButtons[2].onClick.AddListener(() => LoadBattlefieldScene(GameMode.Tutorial));
    }

    public void LoadBattlefieldScene(GameMode mode)
    {
        GameModeManager modeManager = FindObjectOfType<GameModeManager>();
        modeManager.SetGameMode(mode); // Set the game mode

        SceneManager.LoadScene("Battlefield");
    }
}
