using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsunamiSpawner : MonoBehaviour
{
    //Tsunami
    public HashSet<Vector2Int> tsunamiPositions = new HashSet<Vector2Int>();
    [Header("Tsunami Configurations")]
    public GameObject tsunamiPrefab;

    [Header("Grid Settings")]
    private const int TILE_COUNT_X = 6;
    private const int TILE_COUNT_Y = 8;
    private int[] tsunamiValidXCoordinates = { 0, 1, 2, 3, 4, 5 };
    private int tsunamiMinY = 2;
    private int tsunamiMaxY = 5;

    [Header("References")]
    public Transform shipboardTransform;
    private Shipboard shipboard;

    [Header("Dependencies")]
    public ToggleCalamities toggleCalamities;
    public MersenneTwister mt;

    public List<GameObject> activeTsunami = new List<GameObject>();

    public Vector2Int currentTsunamiPosition; // Tracks the tsunami's current position.
    private int tsunamiDirection = 1; // 1 for right, -1 for left.

    private void Awake()
    {
        shipboard = FindObjectOfType<Shipboard>();
        // Seed the Mersenne Twister with the current time in ticks, converted to a 32-bit unsigned integer.
        uint seed = (uint)(DateTime.Now.Ticks & 0xFFFFFFFF);
        mt = new MersenneTwister(seed);
    }

    private void PlaceTsunami(int x, int y)
    {
        tsunamiPositions.Add(new Vector2Int(x, y));
    }

    public void SpawnRandomTsunami()
    {
        // Choose a random side: 0 for leftmost, 1 for rightmost.
        int x = mt.Next(2) == 0 ? 0 : TILE_COUNT_X - 1;

        // Choose a random Y-coordinate within bounds.
        int y;
        do
        {
            y = mt.Next(tsunamiMaxY - tsunamiMinY + 1) + tsunamiMinY;
        } while (IsPositionOccupied(x, y)); // Ensure the position is not occupied.

        SpawnTsunami(x, y); // Spawn the tsunami at the selected position.
    }

    private void SpawnTsunami(int x, int y)
    {
        // Ensure the position is not occupied by a ship.
        while (IsPositionOccupied(x, y))
        {
            // If occupied, pick a new random Y-coordinate within bounds.
            y = mt.Next(tsunamiMaxY - tsunamiMinY + 1) + tsunamiMinY;
        }

        // Instantiate the Pirate Hideout at the specified position.
        GameObject tsunami = Instantiate(tsunamiPrefab, new Vector3(x, 0, y), Quaternion.identity);
        tsunami.transform.SetParent(shipboardTransform);

        // Position the Pirate Hideout correctly in the grid.
        PositionTsunami(tsunami, x, y);

        // Track the instantiated pirate hideout GameObject.
        activeTsunami.Add(tsunami);

        currentTsunamiPosition = new Vector2Int(x, y); // Initialize the position.
        tsunamiDirection = x == 0 ? 1 : -1; // Set direction based on starting side

        // Add the position to the pirate hideout positions set.
        PlaceTsunami(x, y);
    }

    private bool IsPositionOccupied(int x, int y)
    {
        // Check if the position is within bounds and has a ship.
        if (x < 0 || x >= TILE_COUNT_X || y < 0 || y >= TILE_COUNT_Y)
            return true; // Consider out-of-bounds as occupied.

        return shipboard.GetShipboardPieces()[x, y] != null;
    }
    public void PositionTsunami(GameObject tsunami, int x, int y)
    {
        // Calculate the center position of the whirlpool grid cell
        Vector3 tileCenter = shipboard.TileCenter(x, y);

        // Set the position of the whirlpool center prefab
        tsunami.transform.position = tileCenter;
    }

    public void MoveTsunami()
    {
        if (activeTsunami.Count == 0)
        {
            Debug.LogWarning("No active tsunami to move!");
            return;
        }

        // Update the X-coordinate based on the direction.
        int newX = currentTsunamiPosition.x + tsunamiDirection;

        // Check for boundaries: reverse direction if needed.
        if (newX < 0 || newX >= TILE_COUNT_X)
        {
            tsunamiDirection *= -1; // Reverse direction.
            newX = Mathf.Clamp(newX, 0, TILE_COUNT_X - 1); // Ensure within bounds.
        }

        // Update tsunami position in the hash set (if used).
        tsunamiPositions.Remove(currentTsunamiPosition); // Remove old position.
        currentTsunamiPosition = new Vector2Int(newX, currentTsunamiPosition.y);
        tsunamiPositions.Add(currentTsunamiPosition); // Add new position.

        // Check if a ship occupies the target grid.
        ShipPieces ship = shipboard.GetShipAtPosition(newX, currentTsunamiPosition.y);
        if (ship != null)
        {
            // Apply the "Out of Commission" debuff to the ship.
            ship.ApplyOutOfCommissionDebuff(GameManager.instance.turns, "Tsunami");
            Debug.Log($"Tsunami hit {ship.name} at ({newX}, {currentTsunamiPosition.y}). Ship is now Out of Commission.");
        }

        PositionTsunami(activeTsunami[0], currentTsunamiPosition.x, currentTsunamiPosition.y);
    }
}
