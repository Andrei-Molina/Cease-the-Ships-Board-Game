using TMPro;
using UnityEngine;
public enum HandicapType
{
    NoHandicap,
    SubmarineHandicap,
    AircraftCarrierHandicap,
    LightCruiserHandicap
}

public class GameManager : MonoBehaviour
{

    public static GameManager instance; // Singleton instance

    public Sprite player1Avatar; // Store player 1's selected avatar
    public Sprite player2Avatar; // Store player 2's selected avatar

    public string player1Name;
    public string player2Name;

    // Default to no handicap
    public HandicapType? player1Handicap = null;
    public HandicapType? player2Handicap = null;

    public Color player1Color;
    public Color player2Color;
    public Color aiColor;

    public float? selectedTimer;

    public int currentAILevel;

    public bool AI;

    public int turns;
    public int weatherTurns = 3;

    public int player1Turn;
    public int player2Turn;

    public bool enableGameInteraction = true;

    public int player1SkillPoints;
    public int player2SkillPoints;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    public void EnableGameInteraction(bool enable)
    {
        enableGameInteraction = enable;
        Debug.Log($"Game interaction set to: {enable}");
    }

    private float longestGameTime = 0f; // Stores longest recorded game duration

    public void SetLongestGameTime(float gameDuration)
    {
        if (gameDuration > longestGameTime)
        {
            longestGameTime = gameDuration;
            Debug.Log($"New longest game time recorded: {longestGameTime} seconds");
        }
    }

    public float GetLongestGameTime()
    {
        return longestGameTime > 0 ? longestGameTime : 1200f; // Return longest or default time
    }

    public int GetSkillPoints(int team)
    {
        return team == 0 ? player1SkillPoints : player2SkillPoints;
    }

    public string GetPlayerName(int team)
    {
        return team == 0 ? player1Name : player2Name;
    }

}
