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

    // Default to no handicap
    public HandicapType player1Handicap = HandicapType.NoHandicap; 
    public HandicapType player2Handicap = HandicapType.NoHandicap;

    public Color player1Color;
    public Color player2Color;

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
