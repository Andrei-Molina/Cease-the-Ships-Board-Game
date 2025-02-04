using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorScreenManager : MonoBehaviour
{
    //public ErrorScreenManager instance;

    public GameObject errorScreen;
    public TextMeshProUGUI errorText;
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

    public void ShowErrorScreen(int index)
    {
        ClearErrorScreen();
        errorScreen.SetActive(true);
        uiCanvasGroup.interactable = false;

        switch (index)
        {
            case 0:
                errorText.text = "No Avatar Selected.";
                break;
            case 1:
                errorText.text = "No Avatar Name.";
                break;
            case 2:
                errorText.text = "No Handicap Selected.";
                break;
            case 3:
                errorText.text = "No Ships' Color Selected.";
                break;
            case 4:
                errorText.text = "No Game Timer Selected.";
                break;
            case 5:
                errorText.text = "No Game Difficulty Selected.";
                break;
        }
    }

    public void ClearErrorScreen()
    {
        errorScreen.SetActive(false);
        errorText.text = "";
        uiCanvasGroup.interactable = true;
    }
}
