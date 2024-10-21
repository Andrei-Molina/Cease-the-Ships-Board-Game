using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameSettingsManager : MonoBehaviour
{
    public GameObject inGameSettingsCanvas;
    public GameObject inGameSettings; // Reference to the InGameSettings GameObject
    public Button settingsButton;
    public Button settingsXButton;
    public AvatarButtonsManager avatarButtonsManager;
    public GameObject mainMenuPromptScreen;
    public GameObject resignScreen;

    public Button resignButton;
    private VictoryManager victoryManager;

    public GameObject OfferDrawSelf;
    public GameObject OfferDrawEnemy;
    public GameObject OfferDrawDecline;

    private void Start()
    {
        avatarButtonsManager = FindObjectOfType<AvatarButtonsManager>();
        victoryManager = FindObjectOfType<VictoryManager>(); // Find the VictoryManager instance

        settingsButton.onClick.AddListener(OpenInGameSettings);
        resignButton.onClick.AddListener(HandleVictory); // Call the new method
    }

    // Method to load the Main Menu scene
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Deployment");
    }

    // Method to open/close the InGameSettings
    public void OpenInGameSettings()
    {
        ClearUI();
        // Toggle the visibility of the InGameSettings GameObject
        bool isActive = inGameSettings.activeSelf;
        inGameSettingsCanvas.SetActive(!isActive);
        inGameSettings.SetActive(!isActive);

        settingsXButton.onClick.AddListener(CloseInGameSettings);
    }

    public void CloseInGameSettings()
    {
        inGameSettings.SetActive(false);
        settingsButton.interactable = true;

        avatarButtonsManager.playerButtons[4].GetComponent<Button>().interactable = true;
        Image imageComponent = avatarButtonsManager.playerButtons[4].GetComponent<Image>();
        Color color = imageComponent.color; // Get the current color
        color.a = 128 / 255.0f; // Set the alpha value
        imageComponent.color = color; // Assign the modified color back
    }

    public void MainMenuPrompt()
    {
        ClearUI();
        inGameSettingsCanvas.SetActive(true);
        mainMenuPromptScreen.SetActive(true);
    }

    public void ResignScreen()
    {
        ClearUI();
        inGameSettingsCanvas.SetActive(true);
        resignScreen.SetActive(true);
    }
    public void OfferDrawScreenSelf()
    {
        ClearUI();
        inGameSettingsCanvas.SetActive(true);
        OfferDrawSelf.SetActive(true);
    }
    public void OfferDrawScreenEnemy()
    {
        ClearUI();
        inGameSettingsCanvas.SetActive(true);
        OfferDrawEnemy.SetActive(true);
    }
    public void OfferDrawScreenDecline()
    {
        ClearUI();
        inGameSettingsCanvas.SetActive(true);
        OfferDrawDecline.SetActive(true);
    }

    public void ClearUI()
    {
        OfferDrawSelf.SetActive(false);
        OfferDrawEnemy.SetActive(false);
        OfferDrawDecline.SetActive(false);
        resignScreen.SetActive(false);
        inGameSettingsCanvas.SetActive(false);
        inGameSettings.SetActive(false);
        mainMenuPromptScreen.SetActive(false);
    }

    // Method to handle victory based on the current player's turn
    public void HandleVictory()
    {
        ClearUI();
        bool isPlayer1Turn = avatarButtonsManager.shipboard.GetIsPlayer1Turn(); // Check whose turn it is
        int teamToCheckmate = isPlayer1Turn ? 4 : 5; // Set team based on turn

        victoryManager.Checkmate(teamToCheckmate); // Call Checkmate with the appropriate team
    }

    // Method to handle victory based on the current player's turn
    public void HandleDraw()
    {
        ClearUI();

        victoryManager.Checkmate(6); // Call Checkmate with the appropriate team
    }
}
