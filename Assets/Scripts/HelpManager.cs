using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class HelpManager : MonoBehaviour
{
    //public HelpManager instance;

    public GameObject helpScreen;
    public TextMeshProUGUI helpText;
    public CanvasGroup uiCanvasGroup;

    private void Start()
    {
        //If want to use singleton
        /*
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        */
    }

    public void ShowHelpScreen(int index)
    {
        ClearHelpScreen();
        helpScreen.SetActive(true);
        uiCanvasGroup.interactable = false;

        switch (index) 
        {
            case 0: //Main Menu
                helpText.text = "Deploy: Start a battle or begin the game.\r\nLoad Game: Open your saved progress.\r\nHow to Play: View game mechanics and tutorials.\r\nQuit: Close the game.";
                break;
            case 1: //Battle Mode
                helpText.text = "Player vs. Player: Compete against another player.\r\nPlayer vs. Environment: Challenge an AI opponent.";
                break;
            case 2: //Avatar
                helpText.text = "Choose your preferred avatar and enter your\r\ndesired commander's name.";
                break;
            case 3: //Handicap
                helpText.text = "Choose a game piece to be handicapped, which will not be available to you during the game. You can select one from the following options: Submarine, Aircraft Carrier, or Light Cruiser. If you prefer to have all game pieces available, choose No Handicap.";
                break;
            case 4: //Color
                helpText.text = "Choose your preferred game piece color. The selected color will be applied to your board game pieces during the game.";
                break;
            case 5: //Timer
                helpText.text = "Choose your preferred game timer. The selected time will apply to both players.";
                break;
            case 6: //PvE
                helpText.text = "Choose your preferred game difficulty: Easy, Medium, or Hard. Each difficulty has 10 levels to play.";
                break;
            case 7: //Easy, Medium, Hard
                helpText.text = "You can play all 10 levels of this difficulty. To unlock the next level, you must defeat the AI opponent.";
                break;
        }
    }

    public void ClearHelpScreen()
    {
        helpScreen.SetActive(false);
        helpText.text = "";
        uiCanvasGroup.interactable = true;
    }
}
