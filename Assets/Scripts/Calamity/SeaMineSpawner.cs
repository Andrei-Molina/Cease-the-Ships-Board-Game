using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeaMineSpawner : MonoBehaviour
{
    //Sea Mine
    public HashSet<Vector2Int> seaMinePositions = new HashSet<Vector2Int>();
    [Header("Sea Mine Configurations")]
    public GameObject seaMinePrefab;

    [Header("Grid Settings")]
    private const int TILE_COUNT_X = 6;
    private const int TILE_COUNT_Y = 8;
    private int[] seaMineValidXCoordinates = { 0, 1, 2, 3, 4, 5 }; 
    private int seaMineMinY = 2; 
    private int seaMineMaxY = 5; 

    [Header("References")]
    public Transform shipboardTransform;
    private Shipboard shipboard;

    [Header("Dependencies")]
    public ToggleCalamities toggleCalamities;
    public MersenneTwister mt;

    public List<GameObject> activeSeaMine = new List<GameObject>();

    private void Awake()
    {
        shipboard = FindObjectOfType<Shipboard>();
        // Seed the Mersenne Twister with the current time in ticks, converted to a 32-bit unsigned integer.
        uint seed = (uint)(DateTime.Now.Ticks & 0xFFFFFFFF);
        mt = new MersenneTwister(seed);
    }
    private void PlaceSeaMine(int x, int y)
    {
        seaMinePositions.Add(new Vector2Int(x, y));
    }

    public void SpawnRandomSeaMine()
    {
        // Find a random position for the pirate hideout that is not occupied.
        int x, y;
        do
        {
            x = seaMineValidXCoordinates[mt.Next(seaMineValidXCoordinates.Length)];
            y = mt.Next(seaMineMaxY - seaMineMinY + 1) + seaMineMinY;
        }
        while (IsPositionOccupied(x, y)); // Ensure the position is not occupied.

        SpawnSeaMine(x, y); // Spawn the pirate hideout at the unoccupied position.
    }

    private void SpawnSeaMine(int x, int y)
    {
        // Ensure the pirate hideout position is not occupied by a ship.
        while (IsPositionOccupied(x, y))
        {
            // Find a new random position for the pirate hideout.
            x = seaMineValidXCoordinates[mt.Next(seaMineValidXCoordinates.Length)];
            y = mt.Next(seaMineMaxY - seaMineMinY + 1) + seaMineMinY;
        }

        // Instantiate the Pirate Hideout at the specified position.
        GameObject seaMine = Instantiate(seaMinePrefab, new Vector3(x, 0, y), Quaternion.identity);
        seaMine.transform.SetParent(shipboardTransform);

        // Position the Pirate Hideout correctly in the grid.
        PositionSeaMine(seaMine, x, y);

        // Track the instantiated pirate hideout GameObject.
        activeSeaMine.Add(seaMine);

        // Add the position to the pirate hideout positions set.
        PlaceSeaMine(x, y);
    }

    private bool IsPositionOccupied(int x, int y)
    {
        // Check if the position is within bounds and has a ship.
        if (x < 0 || x >= TILE_COUNT_X || y < 0 || y >= TILE_COUNT_Y)
            return true; // Consider out-of-bounds as occupied.

        return shipboard.GetShipboardPieces()[x, y] != null;
    }
    public void PositionSeaMine(GameObject seaMine, int x, int y)
    {
        // Calculate the center position of the whirlpool grid cell
        Vector3 tileCenter = shipboard.TileCenter(x, y);

        // Set the position of the whirlpool center prefab
        seaMine.transform.position = tileCenter;
    }
}
