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
    public GameObject aboutUsScreen;

    [Header("AI Difficulty Screens")]
    public GameObject aiDifficultyScreen;
    public GameObject easyDifficultyScreen;
    public GameObject mediumDifficultyScreen;
    public GameObject hardDifficultyScreen;

    private void Awake() // Awake Instance
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void MainMenu() // Show Main Menu Screen
    {
        ClearUI();
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

        //Clear UI for PVE
        aiDifficultyScreen.SetActive(false);
        easyDifficultyScreen.SetActive(false);
        mediumDifficultyScreen.SetActive(false);
    }
}
