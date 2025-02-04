using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject aiColorScreen;
    public GameObject timerScreen;

    [Header("AI Difficulty Screens")]
    public GameObject aiDifficultyScreen;
    public GameObject easyDifficultyScreen;
    public GameObject mediumDifficultyScreen;
    public GameObject hardDifficultyScreen;

    private HologramManager hologramManager;
    private ColorController colorController;
    private HandicapController handicapController;
    private ErrorScreenManager errorScreenManager;
    public AvatarScreenController avatarScreenController;


    public Button[] DifficultyButtons;
    private int selectedDifficultyIndex = -1; // Track selected difficulty button index

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
        handicapController = FindObjectOfType<HandicapController>();
        avatarScreenController = FindObjectOfType<AvatarScreenController>();
        errorScreenManager = FindObjectOfType<ErrorScreenManager>();
    }

    private void Start()
    {
        // Add listeners to the difficulty buttons
        for (int i = 0; i < DifficultyButtons.Length; i++)
        {
            int index = i; // Capture the index
            DifficultyButtons[i].onClick.AddListener(() => SelectDifficulty(index));
        }
    }

    public void MainMenu() // Show Main Menu Screen
    {
        ClearUI();
        GameManager.instance.AI = false;
        GameManager.instance.player1Avatar = null;
        GameManager.instance.player2Avatar = null;
        GameManager.instance.player1Handicap = null;
        GameManager.instance.player2Handicap = null;
        colorController.ResetAllColorButtonsInteractability();
        handicapController.ResetPlayer1HandicapButtons();
        handicapController.ResetPlayer2HandicapButtons();
        avatarScreenController.ResetPlayer1Values();
        avatarScreenController.ResetPlayer2Values();
        hologramManager.ResetHologramStates(); // Reset hologram states
        mainMenuScreen.SetActive(true);
        SetDifficultyButtonsInteractableToTrue();
    }

    public void BattleModeScreen() // Show Main Menu Screen
    {
        ClearUI();
        GameManager.instance.AI = false;
        selectedDifficultyIndex = -1;
        GameManager.instance.AI = false;
        GameManager.instance.player1Avatar = null;
        GameManager.instance.player2Avatar = null;
        GameManager.instance.player1Handicap = null;
        GameManager.instance.player2Handicap = null;
        colorController.ResetAllColorButtonsInteractability();
        handicapController.ResetPlayer1HandicapButtons();
        handicapController.ResetPlayer2HandicapButtons();
        avatarScreenController.ResetPlayer1Values();
        avatarScreenController.ResetPlayer2Values();
        hologramManager.ResetHologramStates(); // Reset hologram states
        battleModeScreen.SetActive(true);
        SetDifficultyButtonsInteractableToTrue();
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
        if (GameManager.instance.player1Avatar == null)
        {
            errorScreenManager.ShowErrorScreen(0);
            return;
        }

        // Refresh the player name and trim leading/trailing spaces
        GameManager.instance.player1Name = GameManager.instance.player1Name?.Trim();

        string playerName = GameManager.instance.player1Name;

        // Check if the name is null, empty, or only whitespace
        if (string.IsNullOrWhiteSpace(playerName))
        {
            errorScreenManager.ShowErrorScreen(1); // No name entered
            return;
        }

        // Check if there's additional text (the actual name) after the prefix
        string nameAfterPrefix = playerName.Substring(playerName.IndexOf('.') + 1).Trim();
        if (string.IsNullOrWhiteSpace(nameAfterPrefix))
        {
            errorScreenManager.ShowErrorScreen(1); // No valid name after prefix
            return;
        }

        ClearUI();
        playerVsPlayerScreen.SetActive(true);
        p1HandicapScreen.SetActive(true);
    }
    public void Player1ColorScreen()
    {
        if (GameManager.instance.player1Handicap == null)
        {
            errorScreenManager.ShowErrorScreen(2);
            return;
        }

        if (GameManager.instance.player1Handicap != null)
        {
            Debug.Log(GameManager.instance.player1Handicap);
            ClearUI();
            playerVsPlayerScreen.SetActive(true);
            p1ColorScreen.SetActive(true);
            colorController.ResetColorButtonsInteractability();
        }
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
            errorScreenManager.ShowErrorScreen(3);
            Debug.Log("No Color Selected for Player 1");
        }
    }
    public void Player2HandicapScreen()
    {
        if (GameManager.instance.player2Avatar == null)
        {
            errorScreenManager.ShowErrorScreen(0);
            return;
        }

        string playerName = GameManager.instance.player2Name?.Trim();

        // Check if the name is null, empty, or only whitespace
        if (string.IsNullOrWhiteSpace(playerName))
        {
            errorScreenManager.ShowErrorScreen(1); // No name entered
            return;
        }

        // Check if there's additional text (the actual name) after the prefix
        string nameAfterPrefix = playerName.Substring(playerName.IndexOf('.') + 1).Trim();
        if (string.IsNullOrWhiteSpace(nameAfterPrefix))
        {
            errorScreenManager.ShowErrorScreen(1); // No valid name after prefix
            return;
        }

        ClearUI();
        playerVsPlayerScreen.SetActive(true);
        p2HandicapScreen.SetActive(true);
    }
    public void Player2ColorScreen()
    {
        if (GameManager.instance.player2Handicap == null)
        {
            errorScreenManager.ShowErrorScreen(2);
            return;
        }

        if (GameManager.instance.player2Handicap != null)
        {
            ClearUI();
            playerVsPlayerScreen.SetActive(true);
            p2ColorScreen.SetActive(true);
        }
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
            errorScreenManager.ShowErrorScreen(3);
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
        SetDifficultyButtonsInteractableToTrue();
        aiDifficultyScreen.SetActive(true);
    }
    private void EasyDifficultyScreen()
    {
        ClearUI();
        easyDifficultyScreen.SetActive(true);
    }
    private void MediumDifficultyScreen()
    {
        ClearUI();
        mediumDifficultyScreen.SetActive(true);
    }
    private void HardDifficultyScreen()
    {
        ClearUI();
        hardDifficultyScreen.SetActive(true);
    }
    private void AIColorScreen()
    {
        ClearUI();
        playerVsPlayerScreen.SetActive(true);
        aiColorScreen.SetActive(true);
    }
    public void Player1ShipColorDoneClicked()
    {
        if (GameManager.instance.AI)
            AIColorScreen();
        else if (!GameManager.instance.AI)
            Player2AvatarScreen();
    }
    public void SelectDifficulty(int index)
    {
        // If a different button was selected
        if (selectedDifficultyIndex != index)
        {
            // Make the previously selected button interactable again
            if (selectedDifficultyIndex >= 0 && selectedDifficultyIndex < DifficultyButtons.Length)
            {
                DifficultyButtons[selectedDifficultyIndex].interactable = true;
            }

            // Set the new button as non-interactable
            DifficultyButtons[index].interactable = false;

            // Update the selected index
            selectedDifficultyIndex = index;

            Debug.Log("Selected Difficulty: " + index); // For debugging
        }
    }

    // Method called when deploy button is clicked
    public void DeployAIButtonClicked()
    {
        if (selectedDifficultyIndex == -1)
        {
            errorScreenManager.ShowErrorScreen(5);
            return;
        }

        if (selectedDifficultyIndex == 0) // Easy
        {
            EasyDifficultyScreen();
        }
        else if (selectedDifficultyIndex == 1) // Medium
        {
            MediumDifficultyScreen();
        }
        else if (selectedDifficultyIndex == 2) // Hard
        {
            HardDifficultyScreen();
        }

        GameManager.instance.AI = true;
    }

    public void AvatarRetreatButtonClicked()
    {
        if (GameManager.instance.AI)
        {
            if (selectedDifficultyIndex == 0)
                EasyDifficultyScreen();
            else if (selectedDifficultyIndex == 1)
                MediumDifficultyScreen();
            else if (selectedDifficultyIndex == 2)
                HardDifficultyScreen();
        }
        else if (!GameManager.instance.AI)
        {
            GameManager.instance.player1Avatar = null;
            GameManager.instance.player1Name = "";
            BattleModeScreen();
        }
    }

    private void SetDifficultyButtonsInteractableToTrue()
    {
        //Iterate through each button and make them interactable again
        for (int i = 0; i < DifficultyButtons.Length; i++)
            DifficultyButtons[i].interactable = true;
        
        //Reset selectedIndex
        selectedDifficultyIndex = -1;
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
        aiColorScreen.SetActive(false);
        timerScreen.SetActive(false);

        //Clear UI for PVE
        aiDifficultyScreen.SetActive(false);
        easyDifficultyScreen.SetActive(false);
        mediumDifficultyScreen.SetActive(false);
        hardDifficultyScreen.SetActive(false);
    }

    public int GetSelectedDifficultyIndex()
    {
        return selectedDifficultyIndex;
    }
}
