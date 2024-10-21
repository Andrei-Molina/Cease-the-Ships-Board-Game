using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Main Menu Screens")]
    public GameObject mainMenuScreen;
    public GameObject battleModeScreen;
    public GameObject loadGameScreen;
    public GameObject howToPlayScreen;
    public GameObject settingScreen;
    public GameObject settingPanelScreen;
    public GameObject aboutUsScreen;
    public GameObject debugScreen;

    [Header("Player vs Player Screen")]
    public GameObject playerVsPlayerScreen;
    public GameObject p1AvatarScreen;
    public GameObject p1HandicapScreen;
    public GameObject p2AvatarScreen;
    public GameObject p2HandicapScreen;
    public GameObject p1ColorScreen;
    public GameObject p2ColorScreen;
    public GameObject timerScreen;

    [Header("AI Difficulty Screens")]
    public GameObject aiDifficultyScreen;
    public GameObject easyDifficultyScreen;
    public GameObject mediumDifficultyScreen;
    public GameObject hardDifficultyScreen;

    private HologramManager hologramManager;
    private ColorController colorController;

    private void Awake() // Awake Instance
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            DontDestroyOnLoad(this);
        }
        hologramManager = FindObjectOfType<HologramManager>(); // Find the hologram manager
        colorController = FindObjectOfType<ColorController>();
    }

    public void MainMenu() // Show Main Menu Screen
    {
        ClearUI();
        hologramManager.ResetHologramStates(); // Reset hologram states
        mainMenuScreen.SetActive(true);
    }

    public void BattleModeScreen() // Show Main Menu Screen
    {
        ClearUI();
        battleModeScreen.SetActive(true);
    }

    public void LoadScreen()
    {
        ClearUI();
        loadGameScreen.SetActive(true);
    }

    public void HowToPlayScreen()
    {
        ClearUI();
        howToPlayScreen.SetActive(true);
    }

    public void SettingScreen()
    {
        ClearUI();
        settingScreen.SetActive(true);
    }

    public void AboutUsScreen()
    {
        ClearUI();
        aboutUsScreen.SetActive(true);
    }
    public void Player1AvatarScreen()
    {
        ClearUI();
        playerVsPlayerScreen.SetActive(true);
        p1AvatarScreen.SetActive(true);
    }
    public void Player1HandicapScreen()
    {
        ClearUI();
        playerVsPlayerScreen.SetActive(true);
        p1HandicapScreen.SetActive(true);
    }
    public void Player1ColorScreen()
    {
        ClearUI();
        playerVsPlayerScreen.SetActive(true);
        p1ColorScreen.SetActive(true);
    }
    public void Player2AvatarScreen()
    {
        if (colorController.IsPlayer1ColorSelected())
        {
            ClearUI();
            playerVsPlayerScreen.SetActive(true);
            p2AvatarScreen.SetActive(true);
        }
        else
        {
            Debug.Log("No Color Selected for Player 1");
        }
    }
    public void Player2HandicapScreen()
    {
        ClearUI();
        playerVsPlayerScreen.SetActive(true);
        p2HandicapScreen.SetActive(true);
    }
    public void Player2ColorScreen()
    {
        ClearUI();
        playerVsPlayerScreen.SetActive(true);
        p2ColorScreen.SetActive(true);
    }
    public void TimerScreen()
    {
        if (colorController.IsPlayer2ColorSelected())
        {
            ClearUI();
            playerVsPlayerScreen.SetActive(true);
            timerScreen.SetActive(true);
        }
        else
        {
            Debug.Log("No Color Selected for Player 2");
        }
    }
    public void DebugScreen()
    {
        if (debugScreen != null)
        {
            ClearUI();
            debugScreen.gameObject.SetActive(true);
        }
    }
    public void AIDifficultyScreen()
    {
        ClearUI();
        aiDifficultyScreen.SetActive(true);
    }
    public void EasyDifficultyScreen()
    {
        ClearUI();
        easyDifficultyScreen.SetActive(true);
    }
    public void MediumDifficultyScreen()
    {
        ClearUI();
        mediumDifficultyScreen.SetActive(true);
    }
    public void HardDifficultyScreen()
    {
        ClearUI();
        hardDifficultyScreen.SetActive(true);
    }
    private void ClearUI() // Clear all UI
    {
        mainMenuScreen.SetActive(false);
        battleModeScreen.SetActive(false);
        howToPlayScreen.SetActive(false);
        settingScreen.SetActive(false);

        if (debugScreen != null)
            debugScreen.SetActive(false);

        //Clear UI for PVP
        playerVsPlayerScreen.SetActive(false);
      p1AvatarScreen.gameObject.SetActive(false);
        p1HandicapScreen.SetActive(false);
        p2AvatarScreen.gameObject.SetActive(false);
        p2HandicapScreen.SetActive(false);
        p1ColorScreen.SetActive(false);
        p2ColorScreen.SetActive(false);
        timerScreen.SetActive(false);

        //Clear UI for PVE
        aiDifficultyScreen.SetActive(false);
        easyDifficultyScreen.SetActive(false);
        mediumDifficultyScreen.SetActive(false);
        hardDifficultyScreen.SetActive(false);
    }
}
