// Path: AvatarScreenController.cs

using UnityEngine;
using UnityEngine.UI;

public class AvatarScreenController : MonoBehaviour
{
    // Store selected buttons (avatars) for each player
    private Button player1SelectedButton;
    private Button player2SelectedButton;

    // Buttons for player 1 and player 2 avatar selections
    public Button[] player1AvatarButtons;
    public Button[] player2AvatarButtons;

    private void Start()
    {
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
        player1SelectedButton = selectedButton; // Store the button itself
        Image avatarImage = player1SelectedButton.GetComponent<Image>();

        if (avatarImage != null)
        {
            GameManager.instance.player1Avatar = avatarImage.sprite; // Store the sprite in GameManager
            Debug.Log("Player 1 selected avatar: " + selectedButton.name);
        }
    }

    // Function to handle Player 2 avatar selection
    public void SelectPlayer2Avatar(Button selectedButton)
    {
        player2SelectedButton = selectedButton; // Store the button itself
        Image avatarImage = player2SelectedButton.GetComponent<Image>();

        if (avatarImage != null)
        {
            GameManager.instance.player2Avatar = avatarImage.sprite; // Store the sprite in GameManager
            Debug.Log("Player 2 selected avatar: " + selectedButton.name);
        }
    }
}
