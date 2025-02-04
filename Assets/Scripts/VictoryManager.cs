using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeaderboardEntry
{
    public string commanderName;
    public float avgMoveTime;
    public float totalGameTime;
    public int totalPoints;

    public LeaderboardEntry(string commanderName, float avgMoveTime, float totalGameTime, int totalPoints)
    {
        this.commanderName = commanderName;
        this.avgMoveTime = avgMoveTime;
        this.totalGameTime = totalGameTime;
        this.totalPoints = totalPoints;
    }
}



public class VictoryManager : MonoBehaviour
{
    private static VictoryManager instance;
    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private GameObject victoryScreen;

    [SerializeField] private GameObject leaderboardPrefab; // Prefab for leaderboard UI elements
    [SerializeField] private Transform leaderboardParent; // Parent for leaderboard entries

    public GameObject logPrefab; // Prefab containing a TextMeshProUGUI component
    public Transform logParent; // UI container to hold the logs

    private List<string> logEntries = new List<string>(); // Store log entries
    private List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>(); // Stores leaderboard data


    public GameObject leaderboardsScrollView;
    public GameObject logsScrollView;

    public Button leaderboardsButton;
    public Button logsButton;

    public Shipboard shipboard;

    public void AddLog(string logEntry)
    {
        logEntries.Add(logEntry);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persist between scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

        LoadLeaderboard();
    }

    // Victory Logic
    public void Checkmate(int team)
    {
        float avgMoveTime = shipboard.GetAveMoveTime(team);
        float totalGameTime = shipboard.GetTotalGameTime();
        int totalPoints = shipboard.CalculateTotalPoints(team);
        string commanderName = GameManager.instance.GetPlayerName(team);

        // Add and save leaderboard entry
        AddLeaderboardEntry(commanderName, avgMoveTime, totalGameTime, totalPoints);
        SaveLeaderboard();
        DisplayVictory(team);
        DisplayLeaderboard();
    }

    private void AddLeaderboardEntry(string commanderName, float avgMoveTime, float totalGameTime, int totalPoints)
    {
        leaderboardEntries.Add(new LeaderboardEntry(commanderName, avgMoveTime, totalGameTime, totalPoints));

        // Sort by Total Points (Highest First)
        leaderboardEntries.Sort((a, b) => b.totalPoints.CompareTo(a.totalPoints));

        // Keep only Top 10
        if (leaderboardEntries.Count > 10)
        {
            leaderboardEntries.RemoveAt(10);
        }
    }
    private void DisplayLeaderboard()
    {
        // Clear existing leaderboard entries
        foreach (Transform child in leaderboardParent)
        {
            Destroy(child.gameObject);
        }

        // Populate Top 10 Leaderboard
        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            GameObject newEntry = Instantiate(leaderboardPrefab, leaderboardParent);
            TextMeshProUGUI numberText = newEntry.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI nameText = newEntry.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI avgTimeText = newEntry.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timeSpentText = newEntry.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI totalPointsText = newEntry.transform.GetChild(4).GetComponent<TextMeshProUGUI>();

            numberText.text = (i + 1).ToString();
            nameText.text = leaderboardEntries[i].commanderName;
            avgTimeText.text = FormatTime(leaderboardEntries[i].avgMoveTime);
            timeSpentText.text = FormatTime(leaderboardEntries[i].totalGameTime);
            totalPointsText.text = leaderboardEntries[i].totalPoints.ToString();
        }

        Canvas.ForceUpdateCanvases();
    }
    private void SaveLeaderboard()
    {
        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            PlayerPrefs.SetString($"leaderboard_name_{i}", leaderboardEntries[i].commanderName);
            PlayerPrefs.SetFloat($"leaderboard_avgMoveTime_{i}", leaderboardEntries[i].avgMoveTime);
            PlayerPrefs.SetFloat($"leaderboard_totalGameTime_{i}", leaderboardEntries[i].totalGameTime);
            PlayerPrefs.SetInt($"leaderboard_totalPoints_{i}", leaderboardEntries[i].totalPoints);
        }
        PlayerPrefs.SetInt("leaderboard_count", leaderboardEntries.Count);
        PlayerPrefs.Save();
    }

    private void LoadLeaderboard()
    {
        int count = PlayerPrefs.GetInt("leaderboard_count", 0);
        leaderboardEntries.Clear();

        for (int i = 0; i < count; i++)
        {
            string commanderName = PlayerPrefs.GetString($"leaderboard_name_{i}", "Unknown");
            float avgMoveTime = PlayerPrefs.GetFloat($"leaderboard_avgMoveTime_{i}", 0f);
            float totalGameTime = PlayerPrefs.GetFloat($"leaderboard_totalGameTime_{i}", 0f);
            int totalPoints = PlayerPrefs.GetInt($"leaderboard_totalPoints_{i}", 0);

            leaderboardEntries.Add(new LeaderboardEntry(commanderName, avgMoveTime, totalGameTime, totalPoints));
        }
    }
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int sec = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:D2}:{sec:D2}";
    }
    private void DisplayVictory(int winningTeam)
    {
        victoryCanvas.SetActive(true);
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);

        // Clear previous logs if any
        foreach (Transform child in logParent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate logs with numbers
        for (int i = 0; i < logEntries.Count; i++)
        {
            GameObject newLog = Instantiate(logPrefab, logParent);

            // Get references to the TextMeshProUGUI components
            TextMeshProUGUI logNumberText = newLog.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI logText = newLog.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            // Assign values
            logNumberText.text = (i + 1).ToString(); // Log number (1-based index)
            logText.text = logEntries[i]; // Actual log message
        }
    }
    public void OnResetButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void showLeaderboards()
    {
        logsScrollView.gameObject.SetActive(false);
        leaderboardsScrollView.gameObject.SetActive(true);
        SetButtonAlpha(leaderboardsButton, 255);
        SetButtonAlpha(logsButton, 191);
    }

    public void showLogs()
    {
        logsScrollView.gameObject.SetActive(true);
        leaderboardsScrollView.gameObject.SetActive(false);
        SetButtonAlpha(leaderboardsButton, 191);
        SetButtonAlpha(logsButton, 255);
    }

    public void SetButtonAlpha(Button button, float alphaValue)
    {
        alphaValue = Mathf.Clamp(alphaValue, 0, 255) / 255f; // Ensure valid range (0-1)

        // Change the button's image transparency
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color imageColor = buttonImage.color;
            imageColor.a = alphaValue;
            buttonImage.color = imageColor;
        }

        // Change the text transparency (if the button has a TMP_Text component)
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            Color textColor = buttonText.color;
            textColor.a = alphaValue;
            buttonText.color = textColor;
        }
    }

    public void OnExitButton()
    {
        SceneManager.LoadScene("Deployment");
    }
}
