using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AvatarButtonsManager : MonoBehaviour
{
    public Button player1Avatar; // Button for player 1's avatar
    public Button player2Avatar; // Button for player 2's avatar
    public GameObject[] playerButtons; // Array to hold player 1 and player 2's buttons
    public Shipboard shipboard; // Reference to shipboard to check turns
    private Image[] playerButtonImages;


    private bool player1ButtonsVisible = false;
    private bool player2ButtonsVisible = false;

    public GameObject deadShipCanvas;
    public GameObject logsCanvas;
    public GameObject phaseCanvas;
    public GameObject phaseContainer;
    public GameObject phaseBackground;
    public GameObject phaseText;

    // Start is called before the first frame update
    void Start()
    {
        SetButtonsAlpha(0); // Initially hide all buttons

        // Add listeners for player buttons
        for (int i = 0; i < playerButtons.Length; i++)
        {
            int index = i; // Capture the current index
            playerButtons[i].GetComponent<Button>().onClick.AddListener(() => OnPlayerButtonClick(index));
        }

        playerButtonImages = new Image[playerButtons.Length];
        for (int i = 0; i < playerButtons.Length; i++)
        {
            playerButtonImages[i] = playerButtons[i].GetComponent<Image>();
        }
    }
    private void Update()
    {
        // Check the alpha of playerButtons[0]
        float alpha0 = playerButtonImages[0].color.a;
        ExpandableDeadShipController.isExpanded = alpha0 >= 1;
        deadShipCanvas.gameObject.SetActive(alpha0 >= 1);

        // Check the alpha of playerButtons[1]
        float alpha1 = playerButtonImages[1].color.a;
        logsCanvas.gameObject.SetActive(alpha1 >= 1);

        // Check the alpha of playerButtons[2]
        float alpha2 = playerButtonImages[2].color.a;
        phaseContainer.gameObject.SetActive(alpha2 >= 1);

        /*
        // Handle interactivity of avatars
        bool isPhaseCanvasActive = phaseCanvas.activeSelf;
        player1Avatar.interactable = !isPhaseCanvasActive;
        player2Avatar.interactable = !isPhaseCanvasActive;
        */

        // Handle interactivity of avatars based on phase canvas state
        bool isPhaseCanvasActive = phaseBackground.activeSelf && phaseText.activeSelf;
        player1Avatar.interactable = !isPhaseCanvasActive;
        player2Avatar.interactable = !isPhaseCanvasActive;
    }


    // Called when player 1's avatar is clicked
    public void OnPlayer1AvatarClick()
    {
        bool playerTurn = shipboard.GetIsPlayer1Turn();
        if (playerTurn)
        {
            if (player2ButtonsVisible) // Hide Player 2's buttons if visible
            {
                TogglePlayerButtons(ref player2ButtonsVisible);
            }

            phaseCanvas.SetActive(false);
            ResetButtonsAlpha();
            TogglePlayerButtons(ref player1ButtonsVisible);
        }
    }

    // Called when player 2's avatar is clicked
    public void OnPlayer2AvatarClick()
    {
        if (!shipboard.GetIsPlayer1Turn())
        {
            if (player1ButtonsVisible) // Hide Player 1's buttons if visible
            {
                TogglePlayerButtons(ref player1ButtonsVisible);
            }
            phaseCanvas.SetActive(false);
            ResetButtonsAlpha(); 
            TogglePlayerButtons(ref player2ButtonsVisible);
        }
    }

    // Toggle buttons visibility for a specific player
    private void TogglePlayerButtons(ref bool buttonsVisible)
    {
        if (buttonsVisible)
        {
            // Hide buttons if they're currently visible
            SetButtonsAlpha(0);
            buttonsVisible = false;

            foreach (GameObject button in playerButtons)
            {
                button.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            // Show buttons with 128 alpha if hidden
            SetButtonsAlpha(128);
            buttonsVisible = true;
        }
    }

    public void TogglePlayer1Public(ref bool buttonsVisible)
    {
        TogglePlayerButtons(ref player1ButtonsVisible);
    }

    public void TogglePlayer2Public(ref bool buttonsVisible)
    {
        TogglePlayerButtons(ref player2ButtonsVisible);
    }

    // Set alpha of all buttons
    private void SetButtonsAlpha(float alpha)
    {
        foreach (GameObject button in playerButtons)
        {
            SetButtonAlpha(button, alpha);
        }
    }

    public bool GetPlayer1ButtonsVisible()
    {
        return player1ButtonsVisible;
    }

    public bool GetPlayer2ButtonsVisible()
    {
        return player2ButtonsVisible;
    }

    // Set the alpha for a specific button
    private void SetButtonAlpha(GameObject button, float alpha)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = alpha / 255f; // Set alpha as 0-1 range
            buttonImage.color = color;
        }
    }

    // Method to handle button clicks and toggle alpha
    public void OnPlayerButtonClick(int buttonIndex)
    {
        // If button has 255 alpha, do nothing
        if (GetButtonAlpha(playerButtons[buttonIndex]) == 255)
            return;

        // Set the clicked button's alpha to 255
        SetButtonAlpha(playerButtons[buttonIndex], 255);
       
        if (buttonIndex != 3)
        {
            playerButtons[buttonIndex].GetComponent<Button>().interactable = false;
        }

        // Set the rest of the buttons to 128 alpha
        for (int i = 0; i < playerButtons.Length; i++)
        {
            if (i != buttonIndex)
            {
                SetButtonAlpha(playerButtons[i], 128);
                playerButtons[i].GetComponent<Button>().interactable = true;
            }
        }

        if (playerButtons[2].GetComponent<Image>().color.a < 1)
            phaseCanvas.gameObject.SetActive(false);
        if (playerButtons[2].GetComponent<Image>().color.a >= 1)
            phaseCanvas.gameObject.SetActive(true);
    }

    // Get the current alpha of a button
    private float GetButtonAlpha(GameObject button)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            return buttonImage.color.a * 255f; // Convert back to 0-255 range
        }
        return 0;
    }

    // Method to get Image components from playerButtons
    public Image[] GetPlayerButtonImages()
    {
        Image[] buttonImages = new Image[playerButtons.Length];

        for (int i = 0; i < playerButtons.Length; i++)
        {
            buttonImages[i] = playerButtons[i].GetComponent<Image>();
        }

        return buttonImages;
    }

    // Method to check if at least one button image (excluding playerButtons[3]) is active (alpha == 1)
    public bool IsAtLeastOneButtonActive()
    {
        for (int i = 0; i < playerButtonImages.Length; i++)
        {
            // Skip button[3]
            if (i == 3) continue;

            // Check if the button image is not null and alpha is above the threshold
            if (playerButtonImages[i] != null && playerButtonImages[i].color.a >= 0.99f)
            {
                return true;
            }
        }
        return false;
    }

    // New method to reset button alpha values
    private void ResetButtonsAlpha()
    {
        foreach (GameObject button in playerButtons)
        {
            SetButtonAlpha(button, 0); // Set all button alphas to 0
            button.GetComponent<Button>().interactable = true; // Reset interactivity
        }
    }
}
