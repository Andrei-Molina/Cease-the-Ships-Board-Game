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
}
