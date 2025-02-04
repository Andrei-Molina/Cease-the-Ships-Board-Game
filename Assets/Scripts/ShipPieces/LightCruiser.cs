using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCruiser : ShipPieces
{
    [SerializeField] private Sprite burnSkillIcon;
    private List<Vector2Int> highlightedTiles = new List<Vector2Int>();
    private Shipboard shipboard;
    private SkillManager skillManager;

    public int maxHEAmmoBarrageSkillUsage = 3; // Maximum uses of the Burn Skill
    public int remainingHEAmmoBarrageSkillUsage; // Tracks remaining usage

    private Dictionary<ShipPieces, int> burnDebuffTracker = new Dictionary<ShipPieces, int>();

    private void Awake()
    {
        shipboard = FindObjectOfType<Shipboard>();
        skillManager = FindObjectOfType<SkillManager>();
        remainingHEAmmoBarrageSkillUsage = maxHEAmmoBarrageSkillUsage; // Initialize remaining usage
    }
    public override List<Vector2Int> GetAvailableMoves(ref ShipPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int maxRange = isBurned ? 3 : Math.Max(tileCountX, tileCountY); // Restricted to 2 tiles when burned

        // Top Right
        for (int i = 1; i <= maxRange; i++)
        {
            int x = currentX + i;
            int y = currentY + i;
            if (x >= tileCountX || y >= tileCountY)
                break;

            if (board[x, y] == null)
                r.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team &&
                   (board[x, y].type != ShipPieceType.BlueSubmarine &&
                    board[x, y].type != ShipPieceType.RedSubmarine &&
                    board[x, y].type != ShipPieceType.BlackSubmarine &&
                    board[x, y].type != ShipPieceType.SilverSubmarine ||
                    board[x, y].isRevealed)) // Check if submarine is revealed
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }

        // Top Left
        for (int i = 1; i <= maxRange; i++)
        {
            int x = currentX - i;
            int y = currentY + i;
            if (x < 0 || y >= tileCountY)
                break;

            if (board[x, y] == null)
                r.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team &&
                   (board[x, y].type != ShipPieceType.BlueSubmarine &&
                    board[x, y].type != ShipPieceType.RedSubmarine &&
                    board[x, y].type != ShipPieceType.BlackSubmarine &&
                    board[x, y].type != ShipPieceType.SilverSubmarine ||
                    board[x, y].isRevealed)) // Check if submarine is revealed
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }

        // Bottom Right
        for (int i = 1; i <= maxRange; i++)
        {
            int x = currentX + i;
            int y = currentY - i;
            if (x >= tileCountX || y < 0)
                break;

            if (board[x, y] == null)
                r.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team &&
                   (board[x, y].type != ShipPieceType.BlueSubmarine &&
                    board[x, y].type != ShipPieceType.RedSubmarine &&
                    board[x, y].type != ShipPieceType.BlackSubmarine &&
                    board[x, y].type != ShipPieceType.SilverSubmarine ||
                    board[x, y].isRevealed)) // Check if submarine is revealed
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }

        // Bottom Left
        for (int i = 1; i <= maxRange; i++)
        {
            int x = currentX - i;
            int y = currentY - i;
            if (x < 0 || y < 0)
                break;

            if (board[x, y] == null)
                r.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team &&
                   (board[x, y].type != ShipPieceType.BlueSubmarine &&
                    board[x, y].type != ShipPieceType.RedSubmarine &&
                    board[x, y].type != ShipPieceType.BlackSubmarine &&
                    board[x, y].type != ShipPieceType.SilverSubmarine ||
                    board[x, y].isRevealed)) // Check if submarine is revealed
                    r.Add(new Vector2Int(x, y));

                break;
            }
        }

        return r;
    }

    public override List<Skill> GetSkills()
    {
        List<Skill> skills = new List<Skill>
        {
            new Skill
            {
                name = "HE-Ammo Barrage",
                icon = burnSkillIcon, // Set the icon in the Inspector
                Execute = DeployBurnStrike,
                skillPointCost = 5
            }
        };

        return skills;
    }

    private void DeployBurnStrike(ShipPieces ship)
    {
        if (remainingHEAmmoBarrageSkillUsage <= 0)
        {
            Debug.LogWarning("Deploy Burn Strike cannot be used anymore for this Light Cruiser.");
            return;
        }

        Debug.Log($"{ship.name} is preparing a burn strike!");

        skillManager.SkillPanel().gameObject.SetActive(false);

        // Highlight tiles in a cross relative to the ship's position
        HighlightEnemyTilesInSquare();

        // Wait for player to select a target
        StartCoroutine(WaitForBurnStrikeDeployment());
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

    private void HighlightTile(int x, int y)
    {
        ShipPieces targetShip = shipboard.GetShipboardPieces()[x, y];
        if (targetShip != null && targetShip.team != this.team)
        {
            // Highlight the tile
            shipboard.Tiles[x, y].layer = LayerMask.NameToLayer("Highlight");
            Renderer renderer = shipboard.Tiles[x, y].GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = new Color(1f, 0.3f, 0.3f, 0.5f); // Red with transparency

            // Add to highlighted tiles
            highlightedTiles.Add(new Vector2Int(x, y));
        }
    }

    private IEnumerator WaitForBurnStrikeDeployment()
    {
        bool burnStrikeDeployed = false;

        while (!burnStrikeDeployed)
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
                        // Execute burn strike on the selected tile
                        Debug.Log($"Burn strike deployed on tile at position {hitPosition}");
                        ExecuteBurnStrike(hitPosition);

                        foreach (var skill in this.GetSkills())
                        {
                            skillManager.DeductPlayerSkillPoints(this, skill.skillPointCost);
                        }

                        burnStrikeDeployed = true; // Stop waiting for input
                        break;
                    }
                }
                else
                {
                    Debug.Log("Canceled burn strike deployment.");
                    CancelSkillDeployment();
                    break;
                }
            }

            yield return null; // Wait for the next frame
        }

        RemoveHighlightTiles();
    }

    private void ExecuteBurnStrike(Vector2Int position)
    {
        // Get the enemy ship at the target position
        ShipPieces targetShip = shipboard.GetShipboardPieces()[position.x, position.y];
        if (targetShip != null)
        {
            // Apply Burn Debuff
            int currentTurn = shipboard.GetIsPlayer1Turn() ? GameManager.instance.player1Turn : GameManager.instance.player2Turn;
            targetShip.ApplyBurnDebuff(currentTurn, "Burn Strike");

            // Add to tracker for expiry check
            if (!burnDebuffTracker.ContainsKey(targetShip))
                burnDebuffTracker.Add(targetShip, currentTurn);
        }

        // Decrease remaining usage
        remainingHEAmmoBarrageSkillUsage--;

        skillManager.DeselectShip(); // Deselect the ship and close the skill panel

        Debug.Log($"Remaining Deploy Burn Strike uses: {remainingHEAmmoBarrageSkillUsage}");
    }

    private void CancelSkillDeployment()
    {
        Debug.Log("Canceling Burn Strike skill...");
        RemoveHighlightTiles();
        skillManager.DeselectShip(); // Deselect the ship and close the skill panel
    }

    private void RemoveHighlightTiles()
    {
        foreach (var tilePosition in highlightedTiles)
        {
            Renderer renderer = shipboard.Tiles[tilePosition.x, tilePosition.y].GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = Color.white; // Reset to default color

            shipboard.Tiles[tilePosition.x, tilePosition.y].layer = LayerMask.NameToLayer("Tile");
        }

        highlightedTiles.Clear();
    }

    public void CheckForBurnDebuffExpiry()
    {
        List<ShipPieces> expiredDebuffs = new List<ShipPieces>();
        int currentTurn = shipboard.GetIsPlayer1Turn() ? GameManager.instance.player1Turn : GameManager.instance.player2Turn;

        foreach (var entry in burnDebuffTracker)
        {
            ShipPieces ship = entry.Key;
            int debuffTurn = entry.Value;

            // Check if 2 turns have passed since the debuff was applied
            if (currentTurn - debuffTurn >= 2)
            {
                expiredDebuffs.Add(ship);
            }
        }

        // Remove the debuff from expired ships
        foreach (var ship in expiredDebuffs)
        {
            ship.RemoveBurnDebuff();
            burnDebuffTracker.Remove(ship);
            Debug.Log($"{ship.name} is no longer burning.");
        }
    }


}
