using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using TMPro;

public class Destroyer : ShipPieces
{
    [SerializeField] private Sprite deploySmokeIcon;
    private List<Vector2Int> highlightedTiles = new List<Vector2Int>();
    private Shipboard shipboard;
    private SkillManager skillManager;
    public static HashSet<Vector2Int> smokeTiles = new HashSet<Vector2Int>();
    [SerializeField] private GameObject smokePrefab; // Assign this in the Inspector
    private Dictionary<Vector2Int, GameObject> activeSmokeInstances = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, (int player, int turn)> smokePlayerTracker = new Dictionary<Vector2Int, (int player, int turn)>();

    public int maxDeploySmokeUsage = 2; // Maximum number of uses for Deploy Smoke skill
    public int remainingDeploySmokeUsage; // Tracks remaining uses

    public int skillCost = 3;


    private void Awake()
    {
        shipboard = FindObjectOfType<Shipboard>();
        skillManager = FindObjectOfType<SkillManager>();
        remainingDeploySmokeUsage = maxDeploySmokeUsage; // Initialize remaining usage
    }


    /*
    public override List<Vector2Int> GetAvailableMoves(ref ShipPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        int direction = (team == 0) ? 1 : -1;

        //Move One in front
        if (currentY + direction >= 0 && currentY + direction < tileCountY && board[currentX, currentY + direction] == null)
            r.Add(new Vector2Int(currentX, currentY + direction));

        //Move One in down
        if (currentY - direction >= 0 && currentY - direction < tileCountY && board[currentX, currentY - direction] == null)
            r.Add(new Vector2Int(currentX, currentY - direction));

        //Move One in left
        if (currentX - 1 >= 0 && currentX - 1 < tileCountX && board[currentX - 1, currentY] == null)
            r.Add(new Vector2Int(currentX - 1, currentY));

        //Move One in right
        if (currentX + 1 >= 0 && currentX + 1 < tileCountX && board[currentX + 1, currentY] == null)
            r.Add(new Vector2Int(currentX + 1, currentY));

        //Kill Action

        //Both team

        //Kill One in front
        if (currentX != tileCountX && currentY + direction >= 0 && currentY + direction < tileCountY)
        {
            if (board[currentX, currentY + direction] != null && board[currentX, currentY + direction].team != team)
                if (board[currentX, currentY + direction].type != ShipPieceType.BlueSubmarine &&
                    board[currentX, currentY + direction].type != ShipPieceType.RedSubmarine &&
                    board[currentX, currentY + direction].type != ShipPieceType.BlackSubmarine &&
                    board[currentX, currentY + direction].type != ShipPieceType.SilverSubmarine)
                    r.Add(new Vector2Int(currentX, currentY + direction));
        }

        //Kill One in down
        if (currentX != tileCountX && currentY - direction >= 0 && currentY - direction < tileCountY)
            if (board[currentX, currentY - direction] != null && board[currentX, currentY - direction].team != team &&
                (board[currentX, currentY - direction].type != ShipPieceType.BlueSubmarine &&
                board[currentX, currentY - direction].type != ShipPieceType.RedSubmarine &&
                board[currentX, currentY - direction].type != ShipPieceType.BlackSubmarine &&
                board[currentX, currentY - direction].type != ShipPieceType.SilverSubmarine))
                r.Add(new Vector2Int(currentX, currentY - direction));

        //Kill One in left
        if (currentY != tileCountY && currentX - 1 >= 0 && currentX - 1 < tileCountX)
            if (board[currentX - 1, currentY] != null && board[currentX - 1, currentY].team != team &&
                (board[currentX - 1, currentY].type != ShipPieceType.BlueSubmarine &&
                board[currentX - 1, currentY].type != ShipPieceType.RedSubmarine &&
                board[currentX - 1, currentY].type != ShipPieceType.BlackSubmarine &&
                board[currentX - 1, currentY].type != ShipPieceType.SilverSubmarine))
                r.Add(new Vector2Int(currentX - 1, currentY));

        //Kill One in right
        if (currentY != tileCountY && currentX + 1 >= 0 && currentX + 1 < tileCountX)
            if (board[currentX + 1, currentY] != null && board[currentX + 1, currentY].team != team &&
                (board[currentX + 1, currentY].type != ShipPieceType.BlueSubmarine &&
                board[currentX + 1, currentY].type != ShipPieceType.RedSubmarine &&
                board[currentX + 1, currentY].type != ShipPieceType.BlackSubmarine &&
                board[currentX + 1, currentY].type != ShipPieceType.SilverSubmarine))
                r.Add(new Vector2Int(currentX + 1, currentY));

        return r;
    }*/

    public override List<Vector2Int> GetAvailableMoves(ref ShipPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;

        bool isInStartingPosition = (team == 0 && currentY == 1) || (team == 1 && currentY == 6);

        // Set max movement range based on the burn debuff
        int maxMovement = isBurned ? 1 : 2;

        // If the ship has the burn debuff and is NOT in its starting position, it cannot move
        if (isBurned && !isInStartingPosition)
        {
            Debug.Log($"{name} cannot move due to the burn debuff and not being in its starting position.");
            return new List<Vector2Int>(); // Return an empty list
        }

        try
        {
            //Move One in front
            if (currentY + direction >= 0 && currentY + direction < tileCountY && board[currentX, currentY + direction] == null)
                r.Add(new Vector2Int(currentX, currentY + direction));

            //Move One in down
            if (currentY - direction >= 0 && currentY - direction < tileCountY && board[currentX, currentY - direction] == null)
                r.Add(new Vector2Int(currentX, currentY - direction));

            //Move One in left
            if (currentX - 1 >= 0 && currentX - 1 < tileCountX && board[currentX - 1, currentY] == null)
                r.Add(new Vector2Int(currentX - 1, currentY));

            //Move One in right
            if (currentX + 1 >= 0 && currentX + 1 < tileCountX && board[currentX + 1, currentY] == null)
                r.Add(new Vector2Int(currentX + 1, currentY));

            //Two in front
            if (!isBurned && board[currentX, currentY + direction] == null)
            {
                //Player 1 Team
                if (team == 0 && currentY == 1 && board[currentX, currentY + (direction * 2)] == null)
                    r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
                //Player 2 Team
                if (team == 1 && currentY == 6 && board[currentX, currentY + (direction * 2)] == null)
                    r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }

            // Kill move
            if (currentX != tileCountX - 1)
            {
                // Check the ship at the right
                ShipPieces target = board[currentX + 1, currentY + direction];
                if (target != null && target.team != team)
                {
                    if ((target.type != ShipPieceType.BlueSubmarine &&
                         target.type != ShipPieceType.RedSubmarine &&
                         target.type != ShipPieceType.BlackSubmarine &&
                         target.type != ShipPieceType.SilverSubmarine))
                    {
                        r.Add(new Vector2Int(currentX + 1, currentY + direction));
                    }

                    if (target.isRevealed)
                    {
                        r.Add(new Vector2Int(currentX + 1, currentY + direction));
                    }
                }
            }

            if (currentX != 0)
            {
                // Check the ship at the left
                ShipPieces target = board[currentX - 1, currentY + direction];
                if (target != null && target.team != team)
                {
                    if ((target.type != ShipPieceType.BlueSubmarine &&
                         target.type != ShipPieceType.RedSubmarine &&
                         target.type != ShipPieceType.BlackSubmarine &&
                         target.type != ShipPieceType.SilverSubmarine))
                    {
                        r.Add(new Vector2Int(currentX + 1, currentY + direction));
                    }

                    if (target.isRevealed)
                    {
                        r.Add(new Vector2Int(currentX + 1, currentY + direction));
                    }
                }
            }
        }
        catch (IndexOutOfRangeException ex)
        {
            UnityEngine.Debug.LogWarning("Out-of-bounds move detected and ignored.");
        }
        return r;
    }

    public override List<Skill> GetSkills()
    {
        List<Skill> skills = new List<Skill>
        {
            // Add the Deploy Smoke skill
            new Skill
            {
                name = "Deploy Smoke",
                icon = deploySmokeIcon, // Set the icon
                Execute = DeploySmoke,
                skillPointCost = 3
            }
        };

        return skills;
    }

    private void DeploySmoke(ShipPieces ship)
    {
        if (remainingDeploySmokeUsage <= 0)
        {
            Debug.LogWarning("Deploy Smoke cannot be used anymore for this Destroyer.");
            return;
        }

        skillManager.SkillPanel().gameObject.SetActive(false);

        Debug.Log($"{ship.name} is deploying smoke!");
        HighlightAllTiles();
    }

    private void HighlightAllTiles()
    {
        highlightedTiles.Clear();

        // Iterate through all tiles and highlight them
        for (int x = 0; x < shipboard.Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < shipboard.Tiles.GetLength(1); y++)
            {
                if (shipboard.GetShipboardPieces()[x, y] != null && shipboard.GetShipboardPieces()[x, y].team == this.team)
                {
                    shipboard.Tiles[x, y].layer = LayerMask.NameToLayer("Highlight");
                    Renderer renderer = shipboard.Tiles[x, y].GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = new Color(0.5f, 1f, 0.5f, 0.5f); // Green with transparency
                    }

                    // Add the tile position to the highlighted list
                    highlightedTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        // Start listening for clicks on highlighted tiles
        StartCoroutine(WaitForSmokeDeployment());
    }

    private IEnumerator WaitForSmokeDeployment()
    {
        bool smokeDeployed = false;

        while (!smokeDeployed)
        {
            if (Input.GetMouseButtonDown(0)) // Detect left click
            {
                RaycastHit hit;
                Ray ray = shipboard.CurrentCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Highlight")))
                {
                    Vector2Int hitPosition = shipboard.GetTileIndex(hit.transform.gameObject);

                    if (highlightedTiles.Contains(hitPosition))
                    {
                        // Deploy smoke on the clicked tile
                        Debug.Log($"Deploying smoke on tile at position {hitPosition}");
                        PlaceSmokeEffect(hitPosition);

                        foreach (var skill in this.GetSkills())
                        {
                            skillManager.DeductPlayerSkillPoints(this, skill.skillPointCost);
                        }

                        smokeDeployed = true; // Stop waiting for input
                        break;
                    }
                }

                else
                {
                    // Check if the click is outside the gameboard
                    if (Physics.Raycast(ray, out hit) == false || hit.collider == null)
                    {
                        Debug.Log("Skill usage canceled by tapping outside the gameboard.");
                        CancelSkillDeployment();
                        break;
                    }
                }
            }

            yield return null; // Wait for the next frame
        }

        // Remove highlights once the smoke is deployed
        RemoveHighlightTiles();
    }

    private void PlaceSmokeEffect(Vector2Int position)
    {
        // Check if smoke already exists on this tile
        if (activeSmokeInstances.ContainsKey(position))
        {
            Debug.Log($"Smoke already exists at {position}.");
            return;
        }

        // Calculate the world position of the tile using GetTileCenter
        Vector3 tileCenter = shipboard.TileCenter(position.x, position.y);

        // Instantiate the smoke prefab and parent it to the Shipboard
        GameObject smokeInstance = Instantiate(smokePrefab, tileCenter, Quaternion.Euler(90, 0, 0), shipboard.transform);

        // Adjust height (if necessary)
        //smokeInstance.transform.position += Vector3.up * 0.5f; // Adjust upward if prefab aligns with the tile

        // Add the smoke to the tracker
        activeSmokeInstances[position] = smokeInstance;

        // Add the position to the smoke tracker
        smokeTiles.Add(position);

        // Track the player and turn of deployment
        int currentPlayer = shipboard.GetIsPlayer1Turn() ? 1 : 2;
        int currentTurn = currentPlayer == 1 ? GameManager.instance.player1Turn : GameManager.instance.player2Turn;
        smokePlayerTracker[position] = (currentPlayer, currentTurn);

        remainingDeploySmokeUsage--;

        skillManager.DeselectShip();

        // Additional logic for smoke effect can be added here
        Debug.Log($"Smoke effect deployed at {position}");
    }

    // Check if a tile has smoke
    public bool IsTileInSmoke(Vector2Int position)
    {
        return smokeTiles.Contains(position);
    }

    public void CheckForSmokeExpiry()
    {
        List<Vector2Int> expiredSmokes = new List<Vector2Int>();
        int currentPlayer = shipboard.GetIsPlayer1Turn() ? 1 : 2;
        int currentTurn = currentPlayer == 1 ? GameManager.instance.player1Turn : GameManager.instance.player2Turn;

        foreach (var smoke in smokePlayerTracker)
        {
            Vector2Int position = smoke.Key;
            int smokePlayer = smoke.Value.player;
            int smokeTurn = smoke.Value.turn;

            // Check if the smoke was deployed in a previous turn of the same player
            if (smokePlayer == currentPlayer && currentTurn - smokeTurn >= 1)
            {
                expiredSmokes.Add(position);
            }
        }

        // Remove expired smokes
        foreach (var position in expiredSmokes)
        {
            RemoveSmokeEffect(position);
            smokePlayerTracker.Remove(position);
        }
    }


    private void RemoveHighlightTiles()
    {
        foreach (var tilePosition in highlightedTiles)
        {
            Renderer renderer = shipboard.Tiles[tilePosition.x, tilePosition.y].GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.white; // Reset to default color
            }

            shipboard.Tiles[tilePosition.x, tilePosition.y].layer = LayerMask.NameToLayer("Tile");
        }

        highlightedTiles.Clear();
        Debug.Log("Removed all highlights from tiles.");
    }

    // Remove smoke from a tile
    private void RemoveSmokeEffect(Vector2Int position)
    {
        if (activeSmokeInstances.TryGetValue(position, out GameObject smokeInstance))
        {
            // Destroy the smoke prefab instance
            Destroy(smokeInstance);
            activeSmokeInstances.Remove(position);

            // Remove the position from the tracker
            smokeTiles.Remove(position);

            Debug.Log($"Smoke effect removed at {position}.");
        }
    }
    private void CancelSkillDeployment()
    {
        Debug.Log("Canceling Deploy Smoke skill...");
        RemoveHighlightTiles();
        skillManager.DeselectShip(); // Deselect the ship and close the skill panel
    }

}
