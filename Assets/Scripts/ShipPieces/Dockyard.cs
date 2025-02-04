using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dockyard : ShipPieces
{

    [SerializeField] private Sprite repairShipIcon;
    [SerializeField] private Sprite resupplyShipIcon;
    private List<Vector2Int> highlightedTiles = new List<Vector2Int>();
    private Shipboard shipboard;
    private SkillManager skillManager;

    private int useSkill = -1;

    private void Awake()
    {
        shipboard = FindObjectOfType<Shipboard>();
        skillManager = FindObjectOfType<SkillManager>();
    }

    public override List<Skill> GetSkills()
    {
        List<Skill> skills = new List<Skill>
        {
            //Repair Ship Skill
            new Skill
            {
                name = "Repair Ship",
                icon = repairShipIcon, // Set the icon
                Execute = RepairShip,
                skillPointCost = 5
            },

            // Resupply Ship skill
            new Skill
            {
                name = "Resupply Ship",
                icon = resupplyShipIcon,
                Execute = ResupplyShip,
                skillPointCost = 5
            }
        };

        return skills;
    }

    private void RepairShip(ShipPieces ship)
    {
        skillManager.SkillPanel().gameObject.SetActive(false);
        useSkill = 1;

        Debug.Log($"{ship.name} is deploying smoke!");
        HighlightAllTiles();
    }

    private void ResupplyShip(ShipPieces ship)
    {
        skillManager.SkillPanel().gameObject.SetActive(false);
        useSkill = 2;

        Debug.Log("Highlighting allied ships for resupply...");
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
        if (useSkill == 1)
            StartCoroutine(WaitForRepairShipUsage());
        else if (useSkill == 2)
            StartCoroutine(WaitForResupplyUsage());
    }

    private IEnumerator WaitForRepairShipUsage()
    {
        bool skillUsed = false;

        while (!skillUsed)
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
                        // Repair ship at the clicked tile
                        ShipPieces ship = shipboard.GetShipAtPosition(hitPosition.x, hitPosition.y);
                        if (ship != null)
                        {
                            // Remove debuffs if present
                            if (ship.isBurned)
                            {
                                ship.RemoveBurnDebuff();
                            }

                            if (ship.isRevealed)
                            {
                                ship.RemoveRevealedDebuff();
                            }

                            if (ship.isOutOfCommission)
                            {
                                ship.RemoveOutOfCommissionDebuff();
                            }

                            Debug.Log($"Repaired ship at position {hitPosition}");
                        }

                        foreach (var skill in this.GetSkills())
                        {
                            skillManager.DeductPlayerSkillPoints(this, skill.skillPointCost);
                        }

                        skillUsed = true;
                        skillManager.DeselectShip();
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

    private IEnumerator WaitForResupplyUsage()
    {
        bool skillUsed = false;

        while (!skillUsed)
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
                        // Resupply the ship at the clicked position
                        ShipPieces ship = shipboard.GetShipAtPosition(hitPosition.x, hitPosition.y);
                        if (ship != null)
                        {
                            ResupplyShipCharges(ship);
                            Debug.Log($"Resupplied ship at position {hitPosition}");
                        }

                        foreach (var skill in this.GetSkills())
                        {
                            skillManager.DeductPlayerSkillPoints(this, skill.skillPointCost);
                        }

                        skillUsed = true;
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

        // Remove highlights once the skill is used
        RemoveHighlightTiles();
    }
    private void ResupplyShipCharges(ShipPieces ship)
    {
        if (ship is Destroyer destroyer)
        {
            destroyer.remainingDeploySmokeUsage = 2; // Reset smoke usage to 2
        }

        else if (ship is DestroyerASW destroyerASW)
        {
            destroyerASW.remainingDeployDepthChargeUsage = 2; // Reset depth charge to 2
        }

        else if (ship is LightCruiser lightCruiser)
        {
            lightCruiser.remainingHEAmmoBarrageSkillUsage = 3; // Reset HE ammo barrage to 3
        }

        Debug.Log($"{ship.name}'s skill charges have been fully resupplied!");

        skillManager.DeselectShip();
    }

    private void CancelSkillDeployment()
    {
        Debug.Log("Canceling Repair Ship skill...");
        useSkill = -1;
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
        Debug.Log("Removed all highlights from tiles.");
    }
}
