using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandicapController : MonoBehaviour
{
    public Button[] player1HandicapButtons; // New for Player 1's handicap
    public Button[] player2HandicapButtons; // New for Player 2's handicap

    // Start is called before the first frame update
    void Start()
    {
        // Assign functions to handicap buttons
        for (int i = 0; i < player1HandicapButtons.Length; i++)
        {
            int index = i;
            player1HandicapButtons[i].onClick.AddListener(() => SelectPlayer1Handicap((HandicapType)index));
        }

        for (int i = 0; i < player2HandicapButtons.Length; i++)
        {
            int index = i;
            player2HandicapButtons[i].onClick.AddListener(() => SelectPlayer2Handicap((HandicapType)index));
        }
    }
    // New functions for handicap selection
    public void SelectPlayer1Handicap(HandicapType selectedHandicap)
    {
        GameManager.instance.player1Handicap = selectedHandicap;
        Debug.Log("Player 1 selected handicap: " + selectedHandicap);
    }

    public void SelectPlayer2Handicap(HandicapType selectedHandicap)
    {
        GameManager.instance.player2Handicap = selectedHandicap;
        Debug.Log("Player 2 selected handicap: " + selectedHandicap);
    }
}
