using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerASW : ShipPieces
{
    [SerializeField] private Sprite depthChargeIcon;
    private List<Vector2Int> highlightedTiles = new List<Vector2Int>();
    private Shipboard shipboard;
    private SkillManager skillManager;

    public int maxDeployDepthChargeUsage = 2; // Maximum number of uses for Deploy Smoke skill
    public int remainingDeployDepthChargeUsage; // Tracks remaining use

    private Dictionary<ShipPieces, int> revealedDebuffTracker = new Dictionary<ShipPieces, int>();

    private void Awake()
    {
        shipboard = FindObjectOfType<Shipboard>();
        skillManager = FindObjectOfType<SkillManager>();
        remainingDeployDepthChargeUsage = maxDeployDepthChargeUsage; // Initialize remaining usage
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

        //Kill One in front
        if (currentX != tileCountX && currentY + direction >= 0 && currentY + direction < tileCountY)
            if (board[currentX, currentY + direction] != null && board[currentX, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX, currentY + direction));

        //Kill One in down
        if (currentX != tileCountX && currentY - direction >= 0 && currentY - direction < tileCountY)
            if (board[currentX, currentY - direction] != null && board[currentX, currentY - direction].team != team)
                r.Add(new Vector2Int(currentX, currentY - direction));

        //Kill One in left
        if (currentY != tileCountY && currentX - 1 >= 0 && currentX - 1 < tileCountX)
            if (board[currentX - 1, currentY] != null && board[currentX - 1, currentY].team != team)
                r.Add(new Vector2Int(currentX - 1, currentY));

        //Kill One in right
        if (currentY != tileCountY && currentX + 1 >= 0 && currentX + 1 < tileCountX)
            if (board[currentX + 1, currentY] != null && board[currentX + 1, currentY].team != team)
                r.Add(new Vector2Int(currentX + 1, currentY));

        return r;
    }*/
    public override List<Vector2Int> GetAvailableMoves(ref ShipPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // Determine if the ship is in its starting position
        bool isInStartingPosition = (team == 0 && currentY == 1) || (team == 1 && currentY == 6);

        int direction = (team == 0) ? 1 : -1;

        // Set max movement range based on the burn debuff
        int maxMovement = isBurned ? 1 : 2;

        // If the ship has the burn debuff and is NOT in its starting position, it cannot move
        if (isBurned && !isInStartingPosition)
        {
            Debug.Log($"{name} cannot move due to the burn debuff and not being in its starting position.");
            return new List<Vector2Int>(); // Return an empty list
        }

        //One in front
        if (board[currentX, currentY + direction] == null)
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

        // Two in front (only if no burn debuff)
        if (!isBurned && board[currentX, currentY + direction] == null)
        {
            if (team == 0 && currentY == 1 && board[currentX, currentY + (direction * maxMovement)] == null)
                r.Add(new Vector2Int(currentX, currentY + (direction * maxMovement)));
            if (team == 1 && currentY == 6 && board[currentX, currentY + (direction * maxMovement)] == null)
                r.Add(new Vector2Int(currentX, currentY + (direction * maxMovement)));
        }

        //Kill move
        if (currentX != tileCountX - 1)
            if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX + 1, currentY + direction));
        if (currentX != 0)
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX - 1, currentY + direction));

        return r;
    }

    public override List<Skill> GetSkills()
    {
        List<Skill> skills = new List<Skill>
        {
            // Add the Deploy Depth Charge skill
            new Skill
            {
                name = "Deploy Depth Charge",
                icon = depthChargeIcon, // Set the icon in the Inspector
                Execute = DeployDepthCharge,
                skillPointCost = 3
            }
        };

        return skills;
    }

    private void DeployDepthCharge(ShipPieces ship)
    {
        if (remainingDeployDepthChargeUsage <= 0)
        {
            Debug.LogWarning("Deploy Depth Charge cannot be used anymore for this Destroyer.");
            return;
        }

        Debug.Log($"{ship.name} is preparing a depth charge!");

        skillManager.SkillPanel().gameObject.SetActive(false);

        // Highlight tiles in a square relative to the ship's position
        HighlightEnemyTilesInSquare();

        // Wait for player to select a target
        StartCoroutine(WaitForDepthChargeDeployment());
    }

    private void HighlightEnemyTilesInSquare()
    {
        highlightedTiles.Clear();
        int squareRadius = 1; // Adjust square radius as needed

        // Iterate over tiles in the square
        for (int x = currentX - squareRadius; x <= currentX + squareRadius; x++)
        {
            for (int y = currentY - squareRadius; y <= currentY + squareRadius; y++)
            {
                // Skip out-of-bounds tiles
                if (x < 0 || y < 0 || x >= shipboard.Tiles.GetLength(0) || y >= shipboard.Tiles.GetLength(1))
                    continue;

                // Check if the tile contains an enemy ship
                ShipPieces targetShip = shipboard.GetShipboardPieces()[x, y];
                if (targetShip != null && targetShip.team != this.team)
                {
                    // Highlight the tile
                    shipboard.Tiles[x, y].layer = LayerMask.NameToLayer("Highlight");
                    Renderer renderer = shipboard.Tiles[x, y].GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = new Color(1f, 0.5f, 0.5f, 0.5f); // Red with transparency
                    }

                    // Add the tile position to the highlighted list
                    highlightedTiles.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    private IEnumerator WaitForDepthChargeDeployment()
    {
        bool depthChargeDeployed = false;

        while (!depthChargeDeployed)
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
                        // Execute depth charge on the selected tile
                        Debug.Log($"Depth charge deployed on tile at position {hitPosition}");
                        ExecuteDepthCharge(hitPosition);

                        foreach (var skill in this.GetSkills())
                        {
                            skillManager.DeductPlayerSkillPoints(this, skill.skillPointCost);
                        }

                        depthChargeDeployed = true; // Stop waiting for input
                        break;
                    }
                }
                else
                {
                    Debug.Log("Canceled depth charge deployment.");
                    CancelSkillDeployment();
                    break;
                }
            }

            yield return null; // Wait for the next frame
        }

        RemoveHighlightTiles();
    }

    private void ExecuteDepthCharge(Vector2Int position)
    {
        // Get the enemy ship at the target position
        ShipPieces targetShip = shipboard.GetShipboardPieces()[position.x, position.y];
        if (targetShip != null)
        {
            // Apply Revealed Debuff
            int currentTurn = shipboard.GetIsPlayer1Turn() ? GameManager.instance.player1Turn : GameManager.instance.player2Turn;
            targetShip.ApplyRevealedDebuff(currentTurn, "Depth Charge");

            // Add to tracker for expiry check
            if (!revealedDebuffTracker.ContainsKey(targetShip))
            {
                revealedDebuffTracker.Add(targetShip, currentTurn);
            }
        }

        // Decrease remaining usage
        remainingDeployDepthChargeUsage--;

        skillManager.DeselectShip(); // Deselect the ship and close the skill panel

        Debug.Log($"Remaining Deploy Depth Charge uses: {remainingDeployDepthChargeUsage}");
    }

    private void CancelSkillDeployment()
    {
        Debug.Log("Canceling Deploy Depth Charge skill...");
        RemoveHighlightTiles();
        skillManager.DeselectShip(); // Deselect the ship and close the skill panel
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
    }

    public void CheckForRevealedDebuffExpiry()
    {
        List<ShipPieces> expiredDebuffs = new List<ShipPieces>();
        int currentTurn = shipboard.GetIsPlayer1Turn() ? GameManager.instance.player1Turn : GameManager.instance.player2Turn;

        foreach (var entry in revealedDebuffTracker)
        {
            ShipPieces ship = entry.Key;
            int debuffTurn = entry.Value;

            // Check if 1 turn has passed since the debuff was applied
            if (currentTurn - debuffTurn >= 1)
            {
                expiredDebuffs.Add(ship);
            }
        }

        // Remove the debuff from expired ships
        foreach (var ship in expiredDebuffs)
        {
            ship.RemoveRevealedDebuff();
            revealedDebuffTracker.Remove(ship);
            Debug.Log($"{ship.name} is no longer revealed.");
        }
    }

}
