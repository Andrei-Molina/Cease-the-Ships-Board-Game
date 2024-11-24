using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private Dictionary<ShipPieces, bool> pieceThreatCache = new Dictionary<ShipPieces, bool>();
    private Dictionary<string, Dictionary<string, float>> qTable = new Dictionary<string, Dictionary<string, float>>();
    private string qTableFilePath;
    private float learningRate = 0.1f;
    private float discountFactor = 0.9f;
    private float explorationRate = 0.1f;

    // Reference to Shipboard script
    public Shipboard shipboard;

    public bool calamitiesHandled = false; // Flag to check if calamities were handled
    // Track metrics
    private float minScore = 0;
    private float maxScore = 0;

    public int totalTurnsPlayed = 0;
    public AITreeLoader aiTree;
    public bool AI = true;
    // Flag to indicate whether the AI is currently processing its turn
    private bool aiTurnInProgress = false;

    // to manage the game phases
    private GamePhase currentPhase;

    private int totalNodesExplored = 0;
    private int totalMovesEvaluated = 0;
    private float pruningEfficiency = 0;
    private int pruningCount = 0;
    private float bestMoveDecisionTime = 0;

    private float totalReward = 0f;
    private int totalMoves = 0;

    // GamePhase Enum to define game phases
    private enum GamePhase
    {
        StandbyPhase,
        MainPhase1,
        ActionPhase,
        MainPhase2,
        EndPhase
    }
    // Start is called before the first frame update
    void Start()
    {
        // Find the Shipboard component in the scene (adjust if it's on a specific GameObject)
        shipboard = FindObjectOfType<Shipboard>();

        if (shipboard == null)
        {
            //UnityEngine.Debug.LogError("Shipboard script not found in the scene.");
        }

        qTableFilePath = Path.Combine(Application.dataPath, "Resources", "QTable.json");
        LoadQTable();
    }

    void OnDestroy()
    {
        SaveQTable();
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

    // Load the Q-table from a JSON file in Resources
    private void LoadQTable()
    {
        if (File.Exists(qTableFilePath))
        {
            string json = File.ReadAllText(qTableFilePath);
            qTable = JsonUtility.FromJson<SerializableQTable>(json).ToDictionary();
        }
    }

    // Save the Q-table to a JSON file in Resources
    private void SaveQTable()
    {
        SerializableQTable serializableQTable = new SerializableQTable(qTable);
        string json = JsonUtility.ToJson(serializableQTable, true);
        File.WriteAllText(qTableFilePath, json);
    }

    // Convert the board state to a string for use as a Q-table key
    private string GetStateKey()
    {
        ShipPieces[,] pieces = shipboard.GetShipboardPieces();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int x = 0; x < shipboard.GetTileCountX(); x++)
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
                sb.Append(pieces[x, y] != null ? pieces[x, y].GetHashCode().ToString() : "0");
        return sb.ToString();
    }

    // Add these variables globally
    private float totalQValueError = 0f;
    private int qValueUpdateCount = 0;

    // Q-Learning update rule
    private float UpdateQValue(string state, string action, float reward, string nextState)
    {
        if (!qTable.ContainsKey(state))
            qTable[state] = new Dictionary<string, float>();

        if (!qTable[state].ContainsKey(action))
            qTable[state][action] = 0f;

        float maxNextQ = qTable.ContainsKey(nextState) ? qTable[nextState].Values.Max() : 0f;
        float currentQ = qTable[state][action];
        float newQ = currentQ + learningRate * (reward + discountFactor * maxNextQ - currentQ);
        // Log all relevant values for debugging
        UnityEngine.Debug.Log($"State: {state}, Action: {action}");
        UnityEngine.Debug.Log($"Current Q-value: {currentQ}");
        UnityEngine.Debug.Log($"Reward: {reward}");
        UnityEngine.Debug.Log($"Max Q-value of next state: {maxNextQ}");
        UnityEngine.Debug.Log($"Learning Rate: {learningRate}");
        UnityEngine.Debug.Log($"Discount Factor: {discountFactor}");
        UnityEngine.Debug.Log($"Calculated new Q-value: {newQ}");

        /*
        totalReward += reward;
        totalMoves++;

        // Log average reward
        UnityEngine.Debug.Log($"Average Reward Per Move: {totalReward / totalMoves}");

        // Calculate Q-value error
        float qError = Mathf.Abs(newQ - (reward + discountFactor * maxNextQ));
        totalQValueError += qError;
        qValueUpdateCount++;

        // Log Q-value accuracy
        UnityEngine.Debug.Log($"Q-Value Error: {qError}");
        UnityEngine.Debug.Log($"Average Q-Value Error (Accuracy): {totalQValueError / qValueUpdateCount}");
        */

        qTable[state][action] = newQ;

        return newQ;
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

    public IEnumerator AITurn()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        aiTurnInProgress = true;
        yield return new WaitForSeconds(0.5f);
        //UnityEngine.Debug.Log("AITurn being processed");

        if (shipboard.GetIsPlayer1Turn() || !AI) yield break;

        List<ShipPieces> aiPieces = new List<ShipPieces>();
        ShipPieces[,] shipboardPieces = shipboard.GetShipboardPieces();

        for (int x = 0; x < shipboard.GetTileCountX(); x++)
        {
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
            {
                if (shipboardPieces[x, y] != null && shipboardPieces[x, y].team == 1)
                {
                    aiPieces.Add(shipboardPieces[x, y]);
                }
            }
        }

        //float bestScore = float.NegativeInfinity;
        Dictionary<(ShipPieces piece, Vector2Int move), float> moveQValues = new Dictionary<(ShipPieces, Vector2Int), float>();

        foreach (var piece in aiPieces)
        {
            List<Vector2Int> availableMoves = piece.GetAvailableMoves(ref shipboardPieces, shipboard.GetTileCountX(), shipboard.GetTileCountY());
            PreventCheckForAI(piece, ref availableMoves);

            foreach (var move in availableMoves)
            {
                // Calculate move score using Minimax
                float moveScore = MinimaxAlphaBeta(shipboardPieces, depth: 1, alpha: float.NegativeInfinity, beta: float.PositiveInfinity, isMaximizingPlayer: false);

                // Calculate Q-value for the move
                float newQ = UpdateQValue(GetStateKey(), piece.GetHashCode() + "-" + move, moveScore, GetStateKey());
                moveQValues.Add((piece, move), newQ);

                UnityEngine.Debug.Log($"Evaluating move: {piece.type} -> ({move.x}, {move.y}). Evaluation value: {moveScore}, Q-value: {newQ}");
            }
        }

        // Select the moves with the highest Q-value
        if (moveQValues.Count > 0)
        {
            float maxQValue = float.NegativeInfinity;
            foreach (var qValue in moveQValues.Values)
            {
                if (qValue > maxQValue) maxQValue = qValue;
            }

            var bestMoves = moveQValues.Where(kvp => kvp.Value == maxQValue).Select(kvp => kvp.Key).ToList();

            // Randomly choose one of the best moves
            var (selectedPiece, selectedMove) = bestMoves[UnityEngine.Random.Range(0, bestMoves.Count)];
            bool moveSuccessful = shipboard.MoveTo(selectedPiece, selectedMove.x, selectedMove.y, selectedPiece.GetAvailableMoves(ref shipboardPieces, shipboard.GetTileCountX(), shipboard.GetTileCountY()));

            UnityEngine.Debug.Log($"Selected move: {selectedMove} for piece: {selectedPiece}. Q-value of the move: {maxQValue}");

            if (moveSuccessful)
            {
                shipboard.SetIsPlayer1Turn(true);
                aiTurnInProgress = false;
                totalTurnsPlayed++;
            }
        }
        else
        {
            UnityEngine.Debug.Log("AI has no valid moves to make.");
        }

        stopwatch.Stop();
        bestMoveDecisionTime = stopwatch.ElapsedMilliseconds / 1000f;
        //LogMetrics(maxDepth: 3);
    }

    public float MinimaxAlphaBeta(ShipPieces[,] shipboardPieces, int depth, float alpha, float beta, bool isMaximizingPlayer)
    {

        if (depth == 0)
        {
            totalMovesEvaluated++;
            float boardValue = EvaluateBoard(shipboardPieces);
            UnityEngine.Debug.Log($"EvaluateBoard value: {boardValue}");
            return boardValue;
        }

        List<ShipPieces> pieces = new List<ShipPieces>();
        int currentTeam = isMaximizingPlayer ? 0 : 1;

        for (int x = 0; x < shipboard.GetTileCountX(); x++)
        {
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
            {
                if (shipboardPieces[x, y] != null && shipboardPieces[x, y].team == currentTeam)
                    pieces.Add(shipboardPieces[x, y]);
            }
        }

        if (isMaximizingPlayer)
        {
            float maxEval = float.NegativeInfinity;
            foreach (var piece in pieces)
            {
                List<Vector2Int> moves = piece.GetAvailableMoves(ref shipboardPieces, shipboard.GetTileCountX(), shipboard.GetTileCountY());

                int originalX = piece.currentX;
                int originalY = piece.currentY;

                foreach (var move in moves)
                {
                    ShipPieces targetPiece = shipboardPieces[move.x, move.y];
                    shipboardPieces[piece.currentX, piece.currentY] = null;
                    shipboardPieces[move.x, move.y] = piece;
                    piece.currentX = move.x;
                    piece.currentY = move.y;

                    totalNodesExplored++;

                    // Perform Minimax search
                    float eval = MinimaxAlphaBeta(shipboardPieces, depth - 1, alpha, beta, false);

                    // Update Q-value for the state-action pair
                    string stateKey = GetStateKey();
                    string actionKey = piece.GetHashCode() + "-" + move;
                    string nextState = GetStateKey();
                    UpdateQValue(stateKey, actionKey, eval, nextState);

                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);

                    // Undo move
                    piece.currentX = originalX;
                    piece.currentY = originalY;
                    shipboardPieces[move.x, move.y] = targetPiece;
                    shipboardPieces[originalX, originalY] = piece;

                    if (beta <= alpha)
                    {
                        pruningCount++;
                        break;
                    }         
                }
            }
            return maxEval;
        }
        else
        {
            float minEval = float.PositiveInfinity;
            foreach (var piece in pieces)
            {
                List<Vector2Int> moves = piece.GetAvailableMoves(ref shipboardPieces, shipboard.GetTileCountX(), shipboard.GetTileCountY());

                int originalX = piece.currentX;
                int originalY = piece.currentY;

                foreach (var move in moves)
                {
                    ShipPieces targetPiece = shipboardPieces[move.x, move.y];
                    shipboardPieces[piece.currentX, piece.currentY] = null;
                    shipboardPieces[move.x, move.y] = piece;
                    piece.currentX = move.x;
                    piece.currentY = move.y;

                    totalNodesExplored++;

                    // Perform Minimax search
                    float eval = MinimaxAlphaBeta(shipboardPieces, depth - 1, alpha, beta, true);

                    // Update Q-value for the state-action pair
                    string stateKey = GetStateKey();
                    string actionKey = piece.GetHashCode() + "-" + move;
                    string nextState = GetStateKey();
                    UpdateQValue(stateKey, actionKey, eval, nextState);

                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);

                    // Undo move
                    piece.currentX = originalX;
                    piece.currentY = originalY;
                    shipboardPieces[move.x, move.y] = targetPiece;
                    shipboardPieces[originalX, originalY] = piece;

                    if (beta <= alpha)
                    {
                        pruningCount++;
                        break;
                    }
                }
            }
            return minEval;
        }
    }

    // Improved Threat Check with caching to avoid repeated calculations
    private bool IsPieceThreatened(ShipPieces piece, ShipPieces[,] board)
    {
        if (pieceThreatCache.TryGetValue(piece, out bool isThreatened))
            return isThreatened;

        int tileCountX = shipboard.GetTileCountX();
        int tileCountY = shipboard.GetTileCountY();

        foreach (var opponentPiece in GetTeamPieces(board, team: 0)) // opponent team = 0
        {
            foreach (var move in opponentPiece.GetAvailableMoves(ref board, tileCountX, tileCountY))
                if (move.x == piece.currentX && move.y == piece.currentY)
                {
                    pieceThreatCache[piece] = true;
                    return true;
                }
        }

        pieceThreatCache[piece] = false;
        return false;
    }

    private float EvaluateBoard(ShipPieces[,] board)
    {
        float score = 0;
        ShipPieces aiFlagship = null;

        // Locate the AI flagship
        for (int x = 0; x < shipboard.GetTileCountX(); x++)
        {
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
            {
                ShipPieces piece = board[x, y];
                if (piece != null && piece.team == 1 &&
                   (piece.type == ShipPieceType.RedFlagship ||
                    piece.type == ShipPieceType.BlueFlagship ||
                    piece.type == ShipPieceType.BlackFlagship ||
                    piece.type == ShipPieceType.SilverFlagship))
                {
                    aiFlagship = piece;
                    break;
                }
            }
        }

        // Evaluate all pieces and adjust score based on their positioning, risks, and capture opportunities
        for (int x = 0; x < shipboard.GetTileCountX(); x++)
        {
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
            {
                ShipPieces piece = board[x, y];
                if (piece != null)
                {
                    float pieceValue = GetPieceValue(piece);
                    score += (piece.team == 1 ? pieceValue : -pieceValue);

                    // Check if AI pieces are in danger
                    if (piece.team == 1 && IsPieceThreatened(piece, board))
                    {
                        score -= pieceValue * 0.5f; // Apply penalty for AI pieces in danger
                    }

                    // Check if AI flagship is in a threatened position
                    if (piece == aiFlagship && IsPieceThreatened(piece, board))
                    {
                        score -= pieceValue; // Apply additional penalty for flagship being threatened
                    }
                }
            }
        }

        // Additional score adjustments for potential moves (captures and safety)
        foreach (var piece in GetTeamPieces(board, team: 1)) // AI team = 1
        {
            List<Vector2Int> availableMoves = piece.GetAvailableMoves(ref board, shipboard.GetTileCountX(), shipboard.GetTileCountY());
            foreach (var move in availableMoves)
            {
                ShipPieces targetPiece = board[move.x, move.y];
                float valueChange = 0;
                float safetyValue = 0;

                // Check if move captures an opponent piece
                if (targetPiece != null && targetPiece.team != piece.team)
                {
                    valueChange = targetPiece.pieceValue;
                }

                /*
                // Evaluate risk for moving to this position
                bool isRisky = false;
                foreach (var opponentPiece in GetOpponentPieces())
                {
                    List<Vector2Int> opponentMoves = opponentPiece.GetAvailableMoves(ref board, shipboard.GetTileCountX(), shipboard.GetTileCountY());
                    if (opponentMoves.Contains(move))
                    {
                        safetyValue -= piece.pieceValue;
                        isRisky = true;
                    }
                }*/

                // Update score for this move based on potential capture and risk
                score += valueChange + safetyValue;
            }
        }

        return score;
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

    private float GetPieceValue(ShipPieces piece) => piece.pieceValue;

    public void PreventCheckForAI(ShipPieces selectedPiece, ref List<Vector2Int> availableMoves)
    {
        ShipPieces targetFlagship = null;

        for (int x = 0; x < shipboard.GetTileCountX(); x++)
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
                if (shipboard.GetShipboardPieces()[x, y] != null && shipboard.GetShipboardPieces()[x, y].team == selectedPiece.team)
                    if (shipboard.GetShipboardPieces()[x, y].type == ShipPieceType.RedFlagship || shipboard.GetShipboardPieces()[x, y].type == ShipPieceType.BlueFlagship || shipboard.GetShipboardPieces()[x, y].type == ShipPieceType.BlackFlagship || shipboard.GetShipboardPieces()[x, y].type == ShipPieceType.SilverFlagship)
                        targetFlagship = shipboard.GetShipboardPieces()[x, y];

        shipboard.SimulateMoveForSinglePiece(selectedPiece, ref availableMoves, targetFlagship);
    }
    private List<ShipPieces> GetTeamPieces(ShipPieces[,] board, int team)
    {
        List<ShipPieces> pieces = new List<ShipPieces>();
        for (int x = 0; x < shipboard.GetTileCountX(); x++)
            for (int y = 0; y < shipboard.GetTileCountY(); y++)
                if (board[x, y] != null && board[x, y].team == team)
                    pieces.Add(board[x, y]);
        return pieces;
    }

    // Serializable wrapper for saving/loading the Q-table
    [System.Serializable]
    private class SerializableQTable
    {
        public List<StateActionPair> stateActions = new List<StateActionPair>();

        public SerializableQTable(Dictionary<string, Dictionary<string, float>> qTable)
        {
            foreach (var state in qTable)
                foreach (var action in state.Value)
                    stateActions.Add(new StateActionPair(state.Key, action.Key, action.Value));
        }

        public Dictionary<string, Dictionary<string, float>> ToDictionary()
        {
            var dict = new Dictionary<string, Dictionary<string, float>>();
            foreach (var pair in stateActions)
            {
                if (!dict.ContainsKey(pair.state))
                    dict[pair.state] = new Dictionary<string, float>();
                dict[pair.state][pair.action] = pair.qValue;
            }
            return dict;
        }
    }

    [System.Serializable]
    private class StateActionPair
    {
        public string state;
        public string action;
        public float qValue;

        public StateActionPair(string state, string action, float qValue)
        {
            this.state = state;
            this.action = action;
            this.qValue = qValue;
        }
    }

    public void LogMetrics(int maxDepth)
    {
        pruningEfficiency = (float)pruningCount / totalNodesExplored;
        UnityEngine.Debug.Log($"Maximum Depth: {maxDepth}");
        UnityEngine.Debug.Log($"Number of Evaluated Moves: {totalMovesEvaluated}");
        UnityEngine.Debug.Log($"Best Move Decision Time: {bestMoveDecisionTime}s");
        UnityEngine.Debug.Log($"Total Nodes Explored: {totalNodesExplored}");
        UnityEngine.Debug.Log($"Pruning Efficiency: {pruningEfficiency:P}");
    }
}