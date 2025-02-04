// Path: AvatarScreenController.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarScreenController : MonoBehaviour
{
    // Store selected buttons (avatars) for each player
    private Button player1SelectedButton;
    private Button player2SelectedButton;

    // Buttons for player 1 and player 2 avatar selections
    public Button[] player1AvatarButtons;
    public Button[] player2AvatarButtons;
    public Sprite[] AIAvatars;
    public GameObject[] player1AvatarContainers;
    public GameObject[] player2AvatarContainers;
    public TMP_InputField[] player1AvatarInputFields;
    public TMP_InputField[] player2AvatarInputFields;
    public TMP_InputField[] player1AvatarTitles;
    public TMP_InputField[] player2AvatarTitles;

    private void Start()
    {
        // Initialize all player 1 avatar buttons to alpha 128 and set them to interactable
        foreach (var button in player1AvatarButtons)
        {
            SetButtonAlpha(button, 128);
            button.interactable = true;
        }

        // Initialize all player 1 avatar buttons to alpha 128 and set them to interactable
        foreach (var button in player2AvatarButtons)
        {
            SetButtonAlpha(button, 128);
            button.interactable = true;
        }

        // Set character limit for player 1 TMP Input Fields
        foreach (var inputField in player1AvatarInputFields)
        {
            TMP_InputField tmpInputField = inputField.GetComponent<TMP_InputField>();
            if (tmpInputField != null)
            {
                tmpInputField.characterLimit = 9; // Set limit to 9 characters
            }
        }

        // Set character limit for player 1 TMP Input Fields
        foreach (var inputField in player2AvatarInputFields)
        {
            TMP_InputField tmpInputField = inputField.GetComponent<TMP_InputField>();
            if (tmpInputField != null)
            {
                tmpInputField.characterLimit = 9; // Set limit to 9 characters
            }
        }

        // Assign functions to each avatar button for Player 1
        for (int i = 0; i < player1AvatarButtons.Length; i++)
        {
            int index = i; // Capture the correct index
            player1AvatarButtons[i].onClick.AddListener(() => SelectPlayer1Avatar(player1AvatarButtons[index]));
        }

        // Assign functions to each avatar button for Player 2
        for (int i = 0; i < player2AvatarButtons.Length; i++)
        {
            int index = i; // Capture the correct index
            player2AvatarButtons[i].onClick.AddListener(() => SelectPlayer2Avatar(player2AvatarButtons[index]));
        }
    }

    // Function to handle Player 1 avatar selection
    public void SelectPlayer1Avatar(Button selectedButton)
    {
        // Reset previous button if one is selected
        if (player1SelectedButton != null)
        {
            player1SelectedButton.interactable = true;
            SetButtonAlpha(player1SelectedButton, 128);
        }

        // Set the newly selected button
        player1SelectedButton = selectedButton;
        player1SelectedButton.interactable = false;
        SetButtonAlpha(player1SelectedButton, 255);

        // Update GameManager sprite for Player 1
        Image avatarImage = player1SelectedButton.GetComponent<Image>();
        if (avatarImage != null)
        {
            GameManager.instance.player1Avatar = avatarImage.sprite;
            Debug.Log("Player 1 selected avatar: " + selectedButton.name);
        }

        // Show corresponding input field based on the selected avatar
        ShowInputFieldForSelectedAvatar(selectedButton, player1AvatarButtons, player1AvatarContainers, player1AvatarInputFields);
    }

    // Function to handle Player 2 avatar selection
    public void SelectPlayer2Avatar(Button selectedButton)
    {
        // Reset previous button if one is selected
        if (player2SelectedButton != null)
        {
            player2SelectedButton.interactable = true;
            SetButtonAlpha(player2SelectedButton, 128);
        }

        player2SelectedButton = selectedButton; // Store the button itself
        player2SelectedButton.interactable = false;
        SetButtonAlpha(player2SelectedButton, 255);

        Image avatarImage = player2SelectedButton.GetComponent<Image>();
        if (avatarImage != null)
        {
            GameManager.instance.player2Avatar = avatarImage.sprite; // Store the sprite in GameManager
            Debug.Log("Player 2 selected avatar: " + selectedButton.name);
        }

        // Show corresponding input field based on the selected avatar
        ShowInputFieldForSelectedAvatar(selectedButton, player2AvatarButtons, player2AvatarContainers, player2AvatarInputFields);
    }

    // Helper function to set button image alpha
    private void SetButtonAlpha(Button button, float alpha)
    {
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            Color color = image.color;
            color.a = alpha / 255f; // Set alpha value between 0 and 1
            image.color = color;
        }
    }

    // Helper method to show the input field for the selected avatar
    private void ShowInputFieldForSelectedAvatar(Button selectedButton, Button[] avatarButtons, GameObject[] avatarContainers, TMP_InputField[] inputFields)
    {
        // Get the index of the selected button
        int selectedIndex = System.Array.IndexOf(avatarButtons, selectedButton);

        // Hide all input fields first
        foreach (var container in avatarContainers)
        {
            container.SetActive(false);
        }

        // Reset all input fields to empty
        foreach (var inputField in inputFields)
        {
            inputField.text = ""; // Clear the text
        }

        // Show the input field corresponding to the selected avatar
        if (selectedIndex >= 0 && selectedIndex < inputFields.Length)
        {
            avatarContainers[selectedIndex].SetActive(true);
        }
    }

    public void GetPlayer1Name()
    {
        // Find the index of the currently selected button
        int selectedIndex = System.Array.IndexOf(player1AvatarButtons, player1SelectedButton);

        // Check if the selected index is valid
        if (selectedIndex >= 0 && selectedIndex < player1AvatarInputFields.Length)
        {
            // Get the text from the active input field
            string title = player1AvatarTitles[selectedIndex].text;
            string playerName = player1AvatarInputFields[selectedIndex].text;


            // Assign the text to GameManager
            GameManager.instance.player1Name = title + " " + playerName;

            Debug.Log("Player 1 name set to: " + playerName);
        }

        UIManager.instance.Player1HandicapScreen();
    }

    public void GetPlayer2Name()
    {
        // Find the index of the currently selected button
        int selectedIndex = System.Array.IndexOf(player2AvatarButtons, player2SelectedButton);

        // Check if the selected index is valid
        if (selectedIndex >= 0 && selectedIndex < player2AvatarInputFields.Length)
        {
            // Get the text from the active input field
            string title = player2AvatarTitles[selectedIndex].text;
            string playerName = player2AvatarInputFields[selectedIndex].text;


            // Assign the text to GameManager
            GameManager.instance.player2Name = title + " " + playerName;

            Debug.Log("Player 2 name set to: " + playerName);
        }

        UIManager.instance.Player2HandicapScreen();
    }

    public void ResetPlayer1Values()
    {
        // Reset the player 1 name in the GameManager
        GameManager.instance.player1Name = string.Empty; // or null, depending on your preference

        // Reset all player 1 avatar buttons to alpha 128 and set them to interactable
        foreach (var button in player1AvatarButtons)
        {
            button.interactable = true;
            SetButtonAlpha(button, 128);
        }

        // Hide all player 1 avatar containers
        foreach (var container in player1AvatarContainers)
        {
            container.SetActive(false);
        }

        // Clear the input fields
        foreach (var inputField in player1AvatarInputFields)
        {
            inputField.text = string.Empty; // Clear the text
        }

        // Reset the selected button
        player1SelectedButton = null;

        Debug.Log("Player 1 values have been reset.");
    }

    public void ResetPlayer2Values()
    {
        // Reset the player 2 name in the GameManager
        GameManager.instance.player2Name = string.Empty; // or null, depending on your preference

        // Reset all player 2 avatar buttons to alpha 128 and set them to interactable
        foreach (var button in player2AvatarButtons)
        {
            button.interactable = true;
            SetButtonAlpha(button, 128);
        }

        // Hide all player 2 avatar containers
        foreach (var container in player2AvatarContainers)
        {
            container.SetActive(false);
        }

        // Clear the input fields
        foreach (var inputField in player2AvatarInputFields)
        {
            inputField.text = string.Empty; // Clear the text
        }

        // Reset the selected button
        player2SelectedButton = null;

        Debug.Log("Player 2 values have been reset.");
    }

    public void ChangeAvatarIfAI()
    {
        // Check if Player 2 should be controlled by AI
        if (GameManager.instance.AI)
        {
            // Automatically set the avatar based on selectedDifficultyIndex
            int difficultyIndex = UIManager.instance.GetSelectedDifficultyIndex();

            // Ensure the difficulty index is within the bounds of AIAvatars array
            if (difficultyIndex >= 0 && difficultyIndex < AIAvatars.Length)
            {
                GameManager.instance.player2Avatar = AIAvatars[difficultyIndex];
                Debug.Log("Player 2 AI selected avatar based on difficulty: " + difficultyIndex);
            }
            else
            {
                Debug.LogWarning("Selected difficulty index is out of AIAvatars range.");
            }
        }
    }
}
