using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TsunamiSpawner;

public class UnderwaterVolcanoSpawner : MonoBehaviour
{
    //Underwater Volcano
    public HashSet<Vector2Int> underwaterVolcanoPositions = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> lavaPositions = new HashSet<Vector2Int>();
    [Header("Underwater Volcano Configurations")]
    public GameObject underwaterVolcanoPrefab;
    public GameObject lavaPrefab;

    [Header("Grid Settings")]
    private const int TILE_COUNT_X = 6;
    private const int TILE_COUNT_Y = 8;
    private int[] underwaterVolcanoValidXCoordinates = { 0, 1, 2, 3, 4, 5 };
    private int underwaterVolcanoMinY = 2;
    private int underwaterVolcanoMaxY = 5;

    [Header("References")]
    public Transform shipboardTransform;
    private Shipboard shipboard;

    [Header("Dependencies")]
    public ToggleCalamities toggleCalamities;
    public MersenneTwister mt;

    public List<GameObject> activeUnderwaterVolcano = new List<GameObject>();
    public List<LavaData> activeLava = new List<LavaData>(); // List to track lava objects

    public Dictionary<Vector2Int, int> underwaterVolcanoSpawnTurns = new Dictionary<Vector2Int, int>();

    private void Awake()
    {
        shipboard = FindObjectOfType<Shipboard>();
        // Seed the Mersenne Twister with the current time in ticks, converted to a 32-bit unsigned integer.
        uint seed = (uint)(DateTime.Now.Ticks & 0xFFFFFFFF);
        mt = new MersenneTwister(seed);
    }
    private void PlaceUnderwaterVolcano(int x, int y)
    {
        underwaterVolcanoPositions.Add(new Vector2Int(x, y));
    }

    public void SpawnRandomUnderwaterVolcano()
    {
        // Find a random position for the pirate hideout that is not occupied.
        int x, y;
        do
        {
            x = underwaterVolcanoValidXCoordinates[mt.Next(underwaterVolcanoValidXCoordinates.Length)];
            y = mt.Next(underwaterVolcanoMaxY - underwaterVolcanoMinY + 1) + underwaterVolcanoMinY;
        }
        while (IsPositionOccupied(x, y)); // Ensure the position is not occupied.

        SpawnUnderwaterVolcano(x, y); // Spawn the pirate hideout at the unoccupied position.
    }

    private void SpawnUnderwaterVolcano(int x, int y)
    {
        // Ensure the pirate hideout position is not occupied by a ship.
        while (IsPositionOccupied(x, y))
        {
            // Find a new random position for the pirate hideout.
            x = underwaterVolcanoValidXCoordinates[mt.Next(underwaterVolcanoValidXCoordinates.Length)];
            y = mt.Next(underwaterVolcanoMaxY - underwaterVolcanoMinY + 1) + underwaterVolcanoMinY;
        }

        // Instantiate the Underwater Volcano at the specified position.
        GameObject underwaterVolcano = Instantiate(underwaterVolcanoPrefab, new Vector3(x, 0, y), Quaternion.identity);
        underwaterVolcano.transform.SetParent(shipboardTransform);

        // Position the Pirate Hideout correctly in the grid.
        PositionUnderwaterVolcano(underwaterVolcano, x, y);

        // Track the instantiated pirate hideout GameObject.
        activeUnderwaterVolcano.Add(underwaterVolcano);

        // Add the position to the pirate hideout positions set.
        PlaceUnderwaterVolcano(x, y);

        // Store the spawn turn
        underwaterVolcanoSpawnTurns[new Vector2Int(x, y)] = GameManager.instance.turns;
    }

    public Vector2Int GetLastSpawnedUnderwaterVolcanoPosition()
    {
        if (underwaterVolcanoSpawnTurns.Count == 0)
        {
            Debug.LogWarning("No underwater volcanoes have been spawned yet.");
            return new Vector2Int(-1, -1); // Return an invalid position if no volcanoes exist.
        }

        // Retrieve the last position added to the dictionary.
        Vector2Int lastSpawnedPosition = default;
        int lastTurn = int.MinValue;

        foreach (var entry in underwaterVolcanoSpawnTurns)
        {
            if (entry.Value > lastTurn)
            {
                lastSpawnedPosition = entry.Key;
                lastTurn = entry.Value;
            }
        }

        return lastSpawnedPosition;
    }

    private bool IsPositionOccupied(int x, int y)
    {
        // Check if the position is within bounds and has a ship.
        if (x < 0 || x >= TILE_COUNT_X || y < 0 || y >= TILE_COUNT_Y)
            return true; // Consider out-of-bounds as occupied.

        return shipboard.GetShipboardPieces()[x, y] != null;
    }
    public void PositionUnderwaterVolcano(GameObject underwaterVolcano, int x, int y)
    {
        // Calculate the center position of the whirlpool grid cell
        Vector3 tileCenter = shipboard.TileCenter(x, y);

        // Set the position of the whirlpool center prefab
        underwaterVolcano.transform.position = tileCenter;
    }

    // Spawning Lava
    public void SpewLava(Vector2Int volcanoPosition)
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(-1, 0), // Left
            new Vector2Int(0, 1),  // Up
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1)  // Down
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int spawnPosition = volcanoPosition + direction;

            if (spawnPosition.x >= 0 && spawnPosition.x < TILE_COUNT_X && spawnPosition.y >= 0 && spawnPosition.y < TILE_COUNT_Y)
            {
                GameObject lava = Instantiate(lavaPrefab, shipboard.TileCenter(spawnPosition.x, spawnPosition.y), Quaternion.identity);
                lava.transform.SetParent(shipboardTransform);

                // Add lava to tracking list with its direction
                activeLava.Add(new LavaData
                {
                    lavaObject = lava,
                    currentPosition = spawnPosition,
                    direction = direction
                });

                // Add the new position to the lavaPositions HashSet
                lavaPositions.Add(spawnPosition);

                // Check if a ship occupies the target grid
                ShipPieces ship = shipboard.GetShipAtPosition(spawnPosition.x, spawnPosition.y);
                if (ship != null)
                {
                    // Apply the "Out of Commission" debuff to the ship
                    ship.ApplyOutOfCommissionDebuff(GameManager.instance.turns, "Lava");
                    Debug.Log($"Lava hit {ship.name} at ({spawnPosition.x}, {spawnPosition.y}). Ship is now Out of Commission.");
                }
            }
        }
    }

    // Moving Lava
    public void MoveAllLava()
    {
        if (activeLava == null || activeLava.Count == 0)
            return; // No lava to move

        for (int i = activeLava.Count - 1; i >= 0; i--) // Iterate backward for safe removal
        {
            LavaData lava = activeLava[i];
            Vector2Int newPosition = lava.currentPosition + lava.direction;

            // Remove old position from HashSet
            lavaPositions.Remove(lava.currentPosition);

            if (newPosition.x >= 0 && newPosition.x < TILE_COUNT_X && newPosition.y >= 0 && newPosition.y < TILE_COUNT_Y)
            {
                lava.currentPosition = newPosition;
                lava.lavaObject.transform.position = shipboard.TileCenter(newPosition.x, newPosition.y);

                // Add new position to HashSet
                lavaPositions.Add(newPosition);

                // Check if a ship occupies the target grid
                ShipPieces ship = shipboard.GetShipAtPosition(newPosition.x, newPosition.y);
                if (ship != null)
                {
                    // Apply the "Out of Commission" debuff to the ship
                    ship.ApplyOutOfCommissionDebuff(GameManager.instance.turns, "Lava");
                    Debug.Log($"Lava hit {ship.name} at ({newPosition.x}, {newPosition.y}). Ship is now Out of Commission.");
                }
            }
            else
            {
                Destroy(lava.lavaObject); // Destroy if out of bounds
                activeLava.RemoveAt(i);
            }
        }
    }

    // LavaData structure for tracking lava
    public class LavaData
    {
        public GameObject lavaObject;
        public Vector2Int currentPosition;
        public Vector2Int direction;
    }
}
