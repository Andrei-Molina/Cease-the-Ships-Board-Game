using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShipPieceType 
{
    None = 0,
    RedDestroyer = 1,
    RedDestroyerASW = 2,
    RedLightCruiser = 3,
    RedMinelayer = 4,
    RedSubmarine = 5,
    RedAircraftCarrier = 6,
    RedDockyard = 7,
    RedFlagship = 8,
    BlueDestroyer = 9,
    BlueDestroyerASW = 10,
    BlueLightCruiser = 11,
    BlueMinelayer = 12,
    BlueSubmarine = 13,
    BlueAircraftCarrier = 14,
    BlueDockyard = 15,
    BlueFlagship = 16,
    BlackDestroyer = 17,
    BlackDestroyerASW = 18,
    BlackLightCruiser = 19,
    BlackMinelayer = 20,
    BlackSubmarine = 21,
    BlackAircraftCarrier = 22,
    BlackDockyard = 23,
    BlackFlagship = 24,
    SilverDestroyer = 25,
    SilverDestroyerASW = 26,
    SilverLightCruiser = 27,
    SilverMinelayer = 28,
    SilverSubmarine = 29,
    SilverAircraftCarrier = 30,
    SilverDockyard = 31,
    SilverFlagship = 32
}

[System.Serializable]
public class Skill
{
    public string name;
    public Sprite icon;
    public System.Action<ShipPieces> Execute;
    public int skillPointCost;
}

[System.Serializable]
public class ShipPieces : MonoBehaviour
{
    [SerializeField] public int team;
    [SerializeField] public int currentX;
    [SerializeField] public int currentY;
    [SerializeField] public int pieceValue;
    public int turnsInWaterspout;
    [SerializeField] public ShipPieceType type;
    [SerializeField] public Camera shipCamera1;
    [SerializeField] public Camera shipCamera2;

    [SerializeField] private Vector3 desiredPosition;
    [SerializeField] private Vector3 desiredScale = Vector3.one; //new Vector3(1, 1, 1)
    [SerializeField] private bool isMoving = false;
    [SerializeField] private float lerpSpeed = 1000f;

    public bool isOutOfCommission = false;
    public bool isRevealed = false;
    public bool isBurned = false;
    private int outOfCommissionTurn = -1; // Turn when the debuff was applied
    private int revealedTurn = -1;
    private int burnedTurn = -1;
    private string debuffSource = ""; // Tracks the source of the debuff
    private string revealedDebuffSource = "";
    private string burnedDebuffSource = "";

    public bool isFrightened = false;

    private void Start()
    {
        Vector3 rotation;

        switch (type)
        {
            case ShipPieceType.RedAircraftCarrier:
            case ShipPieceType.BlueAircraftCarrier:
            case ShipPieceType.BlackAircraftCarrier:
            case ShipPieceType.SilverAircraftCarrier:
                rotation = (team == 0) ? Vector3.zero : new Vector3(0, -180, 0);
                break;

            case ShipPieceType.RedLightCruiser:
            case ShipPieceType.BlueLightCruiser:
            case ShipPieceType.BlackLightCruiser:
            case ShipPieceType.SilverLightCruiser:
                rotation = (team == 0) ? new Vector3(0, 180, 0) : Vector3.zero;
                break;

            case ShipPieceType.RedFlagship:
            case ShipPieceType.BlueFlagship:
            case ShipPieceType.BlackFlagship:
            case ShipPieceType.SilverFlagship:
                rotation = (team == 0) ? new Vector3(0, 180, 0) : Vector3.zero;
                break;

            case ShipPieceType.RedDockyard:
            case ShipPieceType.BlueDockyard:
            case ShipPieceType.BlackDockyard:
            case ShipPieceType.SilverDockyard:
                rotation = (team == 0) ? Vector3.zero : new Vector3(0, 180, 0);
                break;

            case ShipPieceType.RedDestroyer:
            case ShipPieceType.BlueDestroyer:
            case ShipPieceType.BlackDestroyer:
            case ShipPieceType.SilverDestroyer:
            case ShipPieceType.RedDestroyerASW:
            case ShipPieceType.BlueDestroyerASW:
            case ShipPieceType.BlackDestroyerASW:
            case ShipPieceType.SilverDestroyerASW:
                rotation = (team == 0) ? new Vector3(0, 180, 0) : Vector3.zero;
                break;

            case ShipPieceType.RedSubmarine:
            case ShipPieceType.BlueSubmarine:
            case ShipPieceType.BlackSubmarine:
            case ShipPieceType.SilverSubmarine:
                rotation = (team == 0) ? new Vector3(0, 180, 0) : Vector3.zero;
                break;

            default:
                rotation = (team == 0) ? Vector3.zero : new Vector3(0, 180, 0);
                break;
        }

        transform.rotation = Quaternion.Euler(rotation);
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref ShipPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        return r;
    }

    public virtual List<Skill> GetSkills()
    {
        return new List<Skill>(); // Default implementation returns no skills
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
            transform.position = desiredPosition;
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
            transform.localScale = desiredScale;
    }
    public ShipPieces Clone()
    {
        ShipPieces clone = (ShipPieces)this.MemberwiseClone();
        // Copy any additional deep fields if needed
        return clone;
    }
    public void ApplyOutOfCommissionDebuff(string source = null)
    {
        isOutOfCommission = true;
        debuffSource = source;
        Debug.Log($"{name} is now Out of Commission!");
        // Additional logic for disabling actions, reducing stats, etc.
    }
    public void ApplyOutOfCommissionDebuff(int currentTurn, string source = null)
    {
        isOutOfCommission = true;
        debuffSource = source;
        outOfCommissionTurn = currentTurn;
        Debug.Log($"{name} is now Out of Commission for 2 turns (applied on turn {currentTurn}).");
    }

    public void ApplyRevealedDebuff(int currentTurn, string source = null)
    {
        isRevealed = true;
        revealedDebuffSource = source;
        revealedTurn = currentTurn;
        Debug.Log($"{name} is now Revealed for 2 turns (applied on turn {currentTurn}).");
    }

    public void ApplyBurnDebuff(int currentTurn, string source = null)
    {
        isBurned = true;
        burnedDebuffSource = source;
        burnedTurn = currentTurn;
        Debug.Log($"{name} is now Burned for 2 turns (applied on turn {currentTurn}).");
    }

    public void RemoveOutOfCommissionDebuff()
    {
        isOutOfCommission = false;
        Debug.Log($"{name} is no longer Out of Commission.");
    }

    public void RemoveRevealedDebuff()
    {
        isRevealed = false;
        Debug.Log($"{name} is no longer revealed.");
    }
    public void RemoveBurnDebuff()
    {
        isBurned = false;
        Debug.Log($"{name} is no longer burning.");
    }
    public void CheckAndClearDebuff(int currentTurn)
    {
        if (isOutOfCommission && outOfCommissionTurn >= 0 && currentTurn >= outOfCommissionTurn + 2)
        {
            isOutOfCommission = false;
            outOfCommissionTurn = -1; // Reset the turn tracker
            Debug.Log($"{name} is no longer Out of Commission.");
        }
    }
    public bool IsDebuffFromSource(string source)
    {
        return isOutOfCommission && debuffSource == source;
    }

    public void ApplyFrightenedDebuff(string source = null)
    {
        isFrightened = true;
        debuffSource = source;
        Debug.Log($"{name} is now Frightened by {source ?? "an unknown source"}!");
        // Add additional frightened behavior logic here, e.g., restrict movement.
    }

    public void RemoveFrightenedDebuff()
    {
        isFrightened = false;
        Debug.Log($"{name} is no longer Frightened.");
    }

    public bool IsFrightened()
    {
        return isFrightened;
    }
    public bool IsPlayer1()
    {
        return team == 0; // Returns true if the ship belongs to Player 1
    }
}
