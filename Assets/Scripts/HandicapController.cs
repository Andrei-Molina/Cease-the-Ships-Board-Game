using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandicapController : MonoBehaviour
{
    public Button[] player1HandicapButtons; // New for Player 1's handicap
    public Button[] player2HandicapButtons; // New for Player 2's handicap

    private Button currentPlayer1Button; // Track the currently active button for Player 1
    private Button currentPlayer2Button; // Track the currently active button for Player 2

    // Start is called before the first frame update
    void Start()
    {
        // Assign functions to handicap buttons
        for (int i = 0; i < player1HandicapButtons.Length; i++)
        {
            int index = i;
            player1HandicapButtons[index].onClick.AddListener(() => SelectPlayer1Handicap((HandicapType)index, player1HandicapButtons[index]));
        }

        for (int i = 0; i < player2HandicapButtons.Length; i++)
        {
            int index = i;
            player2HandicapButtons[index].onClick.AddListener(() => SelectPlayer2Handicap((HandicapType)index, player2HandicapButtons[index]));
        }
    }
    // New functions for handicap selection
    public void SelectPlayer1Handicap(HandicapType selectedHandicap, Button clickedButton)
    {
        GameManager.instance.player1Handicap = selectedHandicap;
        Debug.Log("Player 1 selected handicap: " + selectedHandicap);

        // Set the previously active button back to interactable
        if (currentPlayer1Button != null)
        {
            currentPlayer1Button.interactable = true;
        }

        // Set the clicked button to non-interactable and store the reference
        clickedButton.interactable = false;
        currentPlayer1Button = clickedButton;
    }

    public void SelectPlayer2Handicap(HandicapType selectedHandicap, Button clickedButton)
    {
        GameManager.instance.player2Handicap = selectedHandicap;
        Debug.Log("Player 2 selected handicap: " + selectedHandicap);

        // Set the previously active button back to interactable
        if (currentPlayer2Button != null)
        {
            currentPlayer2Button.interactable = true;
        }

        // Set the clicked button to non-interactable and store the reference
        clickedButton.interactable = false;
        currentPlayer2Button = clickedButton;
    }
    public void ResetPlayer1HandicapButtons()
    {
        GameManager.instance.player1Handicap = null;

        // Enable all Player 1 color buttons
        foreach (var button in player1HandicapButtons)
        {
            button.interactable = true;
        }
    }
    public void ResetPlayer2HandicapButtons()
    {
        GameManager.instance.player2Handicap = null;
        // Enable all Player 2 color buttons
        foreach (var button in player2HandicapButtons)
        {
            button.interactable = true;
        }
    }
}
