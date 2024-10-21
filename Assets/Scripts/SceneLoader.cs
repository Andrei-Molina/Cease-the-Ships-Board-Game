using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public Button[] levelButtons; // Easy difficulty level buttons
    public Button[] levelButtonsMedium; // Medium difficulty level buttons
    public Button[] levelButtonsHard;
    public GameObject easyScreen; // The GameObject for the Easy screen (which contains the level buttons)
    public GameObject mediumScreen;
    public GameObject hardScreen;
    public GameObject timerScreen;
    public GameObject howToPlayScreen;
    public Sprite unlockedIcon; // Icon for unlocked levels
    public Sprite lockedIcon;   // Icon for locked levels
    private const string LEVEL_UNLOCK_KEY = "UnlockedLevel"; // PlayerPrefs key to store unlocked easy levels
    private const string LEVEL_UNLOCK_KEY_MEDIUM = "UnlockedLevelMedium"; // PlayerPrefs key to store unlocked medium levels
    private const string LEVEL_UNLOCK_KEY_HARD = "UnlockedLevelHard"; // PlayerPrefs key to store unlocked hard levels
    private bool buttonsUpdated = false; // Flag to track if the buttons have been updated

    //Loading Screen
    [SerializeField] private GameObject loadingScreen; // The loading screen with the slider
    [SerializeField] private Slider loadingSlider; // The slider to fill over time
    [SerializeField] Animator loadingAnimator; // Animator to play "anim_loadship" animation


    void Update()
    {
        // Check if the Easy screen is active
        if ((easyScreen.activeInHierarchy || mediumScreen.activeInHierarchy || hardScreen.activeInHierarchy || timerScreen.activeInHierarchy || howToPlayScreen.activeInHierarchy) && !buttonsUpdated)
        {
            // If the Easy screen is active and the buttons haven't been updated, update them
            UpdateLevelButtons();
            buttonsUpdated = true; // Set flag to true to prevent redundant updates
        }
        else if (!easyScreen.activeInHierarchy || !mediumScreen.activeInHierarchy || !hardScreen.activeInHierarchy)
        {
            // Reset the flag when the screen is inactive
            buttonsUpdated = false;
        }
    }

    // Method to update the interactable state of the level buttons based on unlocked levels
    private void UpdateLevelButtons()
    {
        int unlockedLevel = PlayerPrefs.GetInt(LEVEL_UNLOCK_KEY, 1); // For easy difficulty
        int unlockedLevelMedium = PlayerPrefs.GetInt(LEVEL_UNLOCK_KEY_MEDIUM, 1); // For medium difficulty
        int unlockedLevelHard = PlayerPrefs.GetInt(LEVEL_UNLOCK_KEY_HARD, 1);

        // Update Easy difficulty buttons
        for (int i = 0; i < levelButtons.Length - 2; i++) // Assuming last two buttons are for PvP and Tutorial
        {
            Button button = levelButtons[i];
            Transform iconTransform = button.transform.Find("Icon-Unlocked-Locked");

            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (i + 1 > unlockedLevel)
                {
                    button.interactable = false;
                    iconImage.sprite = lockedIcon; // Set the locked icon
                }
                else
                {
                    int level = i + 1;
                    button.interactable = true;
                    iconImage.sprite = unlockedIcon; // Set the unlocked icon
                    button.onClick.RemoveAllListeners(); // Clear existing listeners
                    button.onClick.AddListener(() => LoadBattlefieldScene(GameMode.PlayerVsEnvironment, level));
                }
            }
        }

        // Update Medium difficulty buttons
        for (int i = 0; i < levelButtonsMedium.Length; i++)
        {
            Button button = levelButtonsMedium[i];
            Transform iconTransform = button.transform.Find("Icon-Unlocked-Locked");

            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (i + 1 > unlockedLevelMedium)
                {
                    button.interactable = false;
                    iconImage.sprite = lockedIcon; // Set the locked icon
                }
                else
                {
                    int level = i + 1;
                    button.interactable = true;
                    iconImage.sprite = unlockedIcon; // Set the unlocked icon
                    button.onClick.RemoveAllListeners(); // Clear existing listeners
                    button.onClick.AddListener(() => LoadBattlefieldScene(GameMode.PlayerVsEnvironmentMedium, level));
                }
            }
        }

        // Update Hard difficulty buttons
        for (int i = 0; i < levelButtonsHard.Length; i++)
        {
            Button button = levelButtonsHard[i];
            Transform iconTransform = button.transform.Find("Icon-Unlocked-Locked");

            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (i + 1 > unlockedLevelHard)
                {
                    button.interactable = false;
                    iconImage.sprite = lockedIcon; // Set the locked icon
                }
                else
                {
                    int level = i + 1;
                    button.interactable = true;
                    iconImage.sprite = unlockedIcon; // Set the unlocked icon
                    button.onClick.RemoveAllListeners(); // Clear existing listeners
                    button.onClick.AddListener(() => LoadBattlefieldScene(GameMode.PlayerVsEnvironmentHard, level));
                }
            }
        }

        levelButtons[levelButtons.Length - 2].onClick.RemoveAllListeners();
        levelButtons[levelButtons.Length - 2].onClick.AddListener(() => LoadBattlefieldScene(GameMode.PlayerVsPlayer));

        levelButtons[levelButtons.Length - 1].onClick.RemoveAllListeners();
        levelButtons[levelButtons.Length - 1].onClick.AddListener(() => LoadBattlefieldScene(GameMode.Tutorial));
    }
    public void LoadBattlefieldScene(GameMode mode, int level = 1)
    {
        if (mode == GameMode.PlayerVsPlayer && GameManager.instance.selectedTimer == null)
        {
            Debug.LogError("Error: Timer not selected! Please select a timer before starting the game.");
            return;
        }

        GameModeManager modeManager = FindObjectOfType<GameModeManager>();
        modeManager.SetGameMode(mode);

        // Set AI level for different difficulties
        if (mode == GameMode.PlayerVsEnvironment || mode == GameMode.PlayerVsEnvironmentMedium || mode == GameMode.PlayerVsEnvironmentHard)
        {
            GameManager.instance.currentAILevel = level;
        }

        // Start the loading coroutine
        StartCoroutine(LoadWithProgress(mode, level));
    }

    // Method to unlock the next level for Easy or Medium difficulty
    public void UnlockNextLevel(int currentLevel, bool isMedium = false, bool isHard = false)
    {
        string unlockKey = isHard ? LEVEL_UNLOCK_KEY_HARD : (isMedium ? LEVEL_UNLOCK_KEY_MEDIUM : LEVEL_UNLOCK_KEY);
        int unlockedLevel = PlayerPrefs.GetInt(unlockKey, 1);
        int maxLevelLength = isHard ? levelButtonsHard.Length : (isMedium ? levelButtonsMedium.Length : levelButtons.Length);

        if (currentLevel >= unlockedLevel && currentLevel < maxLevelLength)
        {
            PlayerPrefs.SetInt(unlockKey, currentLevel + 1); // Unlock the next level
            PlayerPrefs.Save();
            if (isHard)
                levelButtonsHard[currentLevel].interactable = true;
            else if (isMedium)
                levelButtonsMedium[currentLevel].interactable = true; // Enable the button for the next medium level
            else
                levelButtons[currentLevel].interactable = true; // Enable the button for the next easy level
        }
    }

    // Coroutine for simulating loading with slider
    private IEnumerator LoadWithProgress(GameMode mode, int level)
    {
        loadingScreen.SetActive(true); // Activate loading screen
        loadingAnimator.Play("anim_LoadingShip"); // Play the loading animation

        float progress = 0;
        while (progress < 4.5f)
        {
            progress += Time.deltaTime; // Increase progress over time
            loadingSlider.value = progress; // Update slider
            yield return null; // Wait for the next frame
        }

        // Load the scene once loading is done
        SceneManager.LoadScene("Battlefield");
    }
}
