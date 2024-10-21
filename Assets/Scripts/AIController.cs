using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    // Reference to Shipboard script
    public Shipboard shipboard;
    public bool calamitiesHandled = false; // Flag to check if calamities were handled


    // Variables moved to Shipboard
    public int totalTurnsPlayed = 0;
    public AITreeLoader aiTree;
    public bool AI = true;
    // Flag to indicate whether the AI is currently processing its turn
    private bool aiTurnInProgress = false;

    // New variable to manage the game phases
    private GamePhase currentPhase;

    // GamePhase Enum to define game phases
    private enum GamePhase
    {
        StandbyPhase,
        MainPhase1,
        ActionPhase,
        MainPhase2,
        EndPhase
    }

    private Dictionary<string, float> featureWeights = new Dictionary<string, float>
    {
        {"turns", 0.1f},            // Weight for the number of turns
        {"victory_status", -1.0f},  // Weight for victory status (negative if you want to prioritize winning)
        {"increment_code", 0.05f},   // Weight for time increment
        {"opening_eco", 0.2f}       // Weight for ECO
    };

    // Start is called before the first frame update
    void Start()
    {
        // Find the Shipboard component in the scene (adjust if it's on a specific GameObject)
        shipboard = FindObjectOfType<Shipboard>();

        if (shipboard == null)
        {
            Debug.LogError("Shipboard script not found in the scene.");
        }
    }

    // Public method to check if it's AI's turn
    public bool IsAITurn()
    {
        return !shipboard.GetIsPlayer1Turn() && AI;
    }

    // Public method to start AI turn from the Update loop in another script
    public void StartAITurn()
    {
        if (!aiTurnInProgress && IsAITurn())
        {
            StartCoroutine(AIAutoPhaseProgression());
        }
    }

    // The AI's phases progression Coroutine
    public IEnumerator AIAutoPhaseProgression()
    {
        // Start at StandbyPhase
        currentPhase = GamePhase.StandbyPhase;
        yield return new WaitForSeconds(2f); // Wait for 5 seconds

        // Move to MainPhase1
        currentPhase = GamePhase.MainPhase1;
        yield return new WaitForSeconds(2f); // Wait for 5 seconds
        
        // Move to ActionPhase
        currentPhase = GamePhase.ActionPhase;
        StartCoroutine(AITurn());
        yield return new WaitForSeconds(2f); // Wait for 5 seconds
        
        // Move to MainPhase2
        currentPhase = GamePhase.MainPhase2;
        yield return new WaitForSeconds(2f); // Wait for 5 seconds
        
        // Move to EndPhase
        currentPhase = GamePhase.EndPhase;

        // Handle calamities only once
        if (!calamitiesHandled)
        {
            shipboard.HandleCalamities();
            calamitiesHandled = true; // Set the flag to true after handling
        }

        yield return new WaitForSeconds(2f); // Wait for 5 seconds

        // End of AI's turn, switch to Player 1
        shipboard.SetIsPlayer1Turn(true);
        aiTurnInProgress = false; // Reset AI turn flag
        shipboard.changeTurnButton.interactable = true;
    }

    // The AITurn Coroutine moved from the main script
    public IEnumerator AITurn()
    {
        const float ZONE_CONTROL_WEIGHT = 1.5f;

        aiTurnInProgress = true;
        yield return new WaitForSeconds(0.5f);
        Debug.Log("AITurn being processed");

        if (aiTree == null || aiTree.decisionTree == null)
        {
            Debug.LogError("aiTree or decisionTree is not initialized.");
            yield break;
        }

        if (shipboard.GetIsPlayer1Turn() || !AI) yield break;

        List<ShipPieces> aiPieces = new List<ShipPieces>();
        ShipPieces[,] shipboardPieces = shipboard.GetShipboardPieces();

        // Collect AI pieces
        for (int x = 0; x < shipboard.GetTileCountX(); x++)
        {
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
            {
                if (shipboardPieces[x, y] != null && shipboardPieces[x, y].team == 1)
                {
                    aiPieces.Add(shipboardPieces[x, y]);
                    Debug.Log($"Found AI piece: {shipboardPieces[x, y].name} at ({x}, {y})");
                }
            }
        }

        if (aiPieces.Count > 0)
        {
            List<(ShipPieces piece, Vector2Int move, List<Vector2Int> availableMoves)> bestMoves = new List<(ShipPieces, Vector2Int, List<Vector2Int>)>();
            float bestOutcome = float.MinValue;

            foreach (ShipPieces selectedPiece in aiPieces)
            {
                Vector2Int previousPosition = new Vector2Int(selectedPiece.currentX, selectedPiece.currentY);
                List<Vector2Int> availableMoves = selectedPiece.GetAvailableMoves(ref shipboardPieces, shipboard.GetTileCountX(), shipboard.GetTileCountY());
                Debug.Log($"Current piece: {selectedPiece.name} at position ({selectedPiece.currentX}, {selectedPiece.currentY}) with available moves: {string.Join(", ", availableMoves)}");
                // Prevent AI from moving into a position that puts it in check
                PreventCheckForAI(selectedPiece, ref availableMoves);

                foreach (Vector2Int moveToPosition in availableMoves)
                {
                    ShipPieces targetPiece = shipboardPieces[moveToPosition.x, moveToPosition.y];
                    float valueChange = 0;
                    float safetyValue = 0;

                    if (targetPiece != null && targetPiece.team != selectedPiece.team)
                    {
                        valueChange = targetPiece.pieceValue;
                        Debug.Log($"Evaluating capture of opponent piece: {targetPiece.name} with value {valueChange}");
                    }

                    foreach (var opponentPiece in GetOpponentPieces())
                    {
                        var opponentMoves = opponentPiece.GetAvailableMoves(ref shipboardPieces, shipboard.GetTileCountX(), shipboard.GetTileCountY());
                        if (opponentMoves.Contains(moveToPosition))
                        {
                            safetyValue -= selectedPiece.pieceValue;
                            Debug.Log($"Move to {moveToPosition} is unsafe due to opponent piece: {opponentPiece.name}");
                        }
                    }

                    var boardState = new Dictionary<string, float>
                {
                    {"turns", GetTurnNumber() * featureWeights["turns"]},
                    {"victory_status", shipboard.GetIsPlayer1Turn() ? 0f : 1f * featureWeights["victory_status"]},
                    {"increment_code", CalculateIncrementCode() * featureWeights["increment_code"]},
                    {"opening_eco", CalculateOpeningECO() * featureWeights["opening_eco"]}
                };

                    float outcome = (aiTree.EvaluateUsingTree(boardState) * 2.5f) + valueChange + safetyValue + (ZONE_CONTROL_WEIGHT * CountControlledZones());
                    Debug.Log($"Evaluating move for {selectedPiece.name} to {moveToPosition} yielded outcome: {outcome}");

                    float worstOpponentOutcome = float.MaxValue;
                    foreach (ShipPieces opponentPiece in GetOpponentPieces())
                    {
                        var opponentMoves = opponentPiece.GetAvailableMoves(ref shipboardPieces, shipboard.GetTileCountX(), shipboard.GetTileCountY());
                        foreach (Vector2Int opponentMove in opponentMoves)
                        {
                            var opponentBoardState = new Dictionary<string, float>
                        {
                            {"turns", GetTurnNumber() * featureWeights["turns"]},
                            {"victory_status", shipboard.GetIsPlayer1Turn() ? 1f : 0f * featureWeights["victory_status"]},
                            {"increment_code", CalculateIncrementCode() * featureWeights["increment_code"]},
                            {"opening_eco", CalculateOpeningECO() * featureWeights["opening_eco"]}
                        };

                            ShipPieces opponentTargetPiece = shipboardPieces[opponentMove.x, opponentMove.y];
                            float opponentValueChange = 0;
                            float opponentSafetyValue = 0;

                            if (opponentTargetPiece != null && opponentTargetPiece.team != opponentPiece.team)
                            {
                                opponentValueChange = opponentTargetPiece.pieceValue;
                            }

                            foreach (var aiPiece in aiPieces)
                            {
                                var aiPieceMoves = aiPiece.GetAvailableMoves(ref shipboardPieces, shipboard.GetTileCountX(), shipboard.GetTileCountY());
                                if (aiPieceMoves.Contains(opponentMove))
                                {
                                    opponentSafetyValue -= opponentPiece.pieceValue;
                                }
                            }

                            float opponentOutcome = aiTree.EvaluateUsingTree(opponentBoardState) + opponentValueChange + opponentSafetyValue;
                            worstOpponentOutcome = Mathf.Min(worstOpponentOutcome, opponentOutcome);
                        }
                    }

                    outcome -= worstOpponentOutcome;

                    Debug.Log($"Adjusted outcome for move {moveToPosition}: {outcome} (after considering worst opponent outcome)");

                    if (outcome > bestOutcome)
                    {
                        bestOutcome = outcome;
                        bestMoves.Clear();
                        bestMoves.Add((selectedPiece, moveToPosition, availableMoves));
                        Debug.Log($"New best outcome found: {bestOutcome} for move to {moveToPosition}");
                    }
                    else if (outcome == bestOutcome)
                    {
                        bestMoves.Add((selectedPiece, moveToPosition, availableMoves));
                    }
                }
            }

            if (bestMoves.Count > 0)
            {
                var (bestPiece, bestMove, availableMoves) = bestMoves[Random.Range(0, bestMoves.Count)];

                Debug.Log($"AI selected piece at {bestPiece.currentX}, {bestPiece.currentY} to move to {bestMove.x}, {bestMove.y} with outcome {bestOutcome}");
                bool moveSuccessful = shipboard.MoveTo(bestPiece, bestMove.x, bestMove.y, availableMoves);

                Debug.Log($"Best move executed to ({bestMove.x}, {bestMove.y}). Move result: {moveSuccessful}");

                if (moveSuccessful)
                {
                    shipboard.SetIsPlayer1Turn(true);
                    aiTurnInProgress = false;
                    totalTurnsPlayed++;

                }
                else
                {
                    Debug.LogWarning("MoveTo method failed.");
                }
            }
            else
            {
                Debug.Log("AI couldn't find any valid moves.");
            }
        }
    }



    // Counts and returns the remaining ships for the Player (team 0)
    private List<ShipPieces> GetOpponentPieces()
    {
        List<ShipPieces> opponentPieces = new List<ShipPieces>();

        for (int x = 0; x < shipboard.GetTileCountX(); x++)
        {
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
            {
                if (shipboard.GetShipboardPieces()[x, y] != null && shipboard.GetShipboardPieces()[x, y].team == 0)
                {
                    opponentPieces.Add(shipboard.GetShipboardPieces()[x, y]);
                }
            }
        }

        return opponentPieces;
    }

    // Counts the remaining ships for the AI (team 1)
    private int CountAIShips()
    {
        int aiShipCount = 0;

        for (int x = 0; x < shipboard.GetTileCountX(); x++)
        {
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
            {
                if (shipboard.GetShipboardPieces()[x, y] != null && shipboard.GetShipboardPieces()[x, y].team == 1)
                {
                    aiShipCount++;
                }
            }
        }

        return aiShipCount;
    }

    // Counts the remaining ships for the Player (team 2)
    private int CountPlayerShips()
    {
        int playerShipCount = 0;

        for (int x = 0; x < shipboard.GetTileCountX(); x++)
        {
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
            {
                if (shipboard.GetShipboardPieces()[x, y] != null && shipboard.GetShipboardPieces()[x, y].team == 0)
                {
                    playerShipCount++;
                }
            }
        }

        return playerShipCount;
    }

    private const int TOTAL_ZONES = 8; // Example value, adjust based on your game

    // Count how many important zones are controlled by AI (team 1)
    private int CountControlledZones()
    {
        int controlledZoneCount = 0;

        // Define key positions on the board that are strategically important (customize based on your game's grid)
        List<Vector2Int> importantZones = new List<Vector2Int>
    {
        new Vector2Int(1, 2),  // Example positions
        new Vector2Int(2, 2),  // Add as many key positions as necessary
        new Vector2Int(3, 2),
        new Vector2Int(4, 2),
        new Vector2Int(2, 3),
        new Vector2Int(2, 4),
        new Vector2Int(3, 3),
        new Vector2Int(3, 4)
    };

        // Iterate through the list of key positions
        foreach (Vector2Int zone in importantZones)
        {
            // Check if the AI (team 1) controls the zone
            ShipPieces piece = shipboard.GetShipboardPieces()[zone.x, zone.y];
            if (piece != null && piece.team == 1)
            {
                controlledZoneCount++;
            }
        }

        return controlledZoneCount; // Return the number of zones controlled by AI
    }
    private int GetTurnNumber()
    {
        // Example method to return the current turn number
        return totalTurnsPlayed;
    }

    // Example method for abstract evaluation of the game state
    private float CalculateIncrementCode()
    {
        // As an example, increment this value based on AI's piece count vs player's piece count
        int aiShips = CountAIShips();
        int playerShips = CountPlayerShips();

        return (aiShips - playerShips) + 10f; // Adjust based on your game's logic
    }

    private float CalculateOpeningECO()
    {
        // You can use this to represent board control. For example, count how many of the most important positions are occupied.
        int controlledZones = CountControlledZones();
        return controlledZones / (float)TOTAL_ZONES; // Normalize the value based on the total zones
    }
    public void PreventCheckForAI(ShipPieces selectedPiece, ref List<Vector2Int> availableMoves)
    {
        ShipPieces targetFlagship = null;

        // Identify the AI's flagship
        for (int x = 0; x < shipboard.GetTileCountX(); x++)
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
                if (shipboard.GetShipboardPieces()[x, y] != null && shipboard.GetShipboardPieces()[x, y].team == selectedPiece.team)
                    if (shipboard.GetShipboardPieces()[x, y].type == ShipPieceType.RedFlagship || shipboard.GetShipboardPieces()[x, y].type == ShipPieceType.BlueFlagship || shipboard.GetShipboardPieces()[x, y].type == ShipPieceType.BlackFlagship || shipboard.GetShipboardPieces()[x, y].type == ShipPieceType.SilverFlagship)
                        targetFlagship = shipboard.GetShipboardPieces()[x, y];

        // Simulate moves and remove any that would put the AI in check
        shipboard.SimulateMoveForSinglePiece(selectedPiece, ref availableMoves, targetFlagship);
    }
}
