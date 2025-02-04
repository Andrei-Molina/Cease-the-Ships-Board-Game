using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipwreckAreaSpawner : MonoBehaviour
{
    //Shipwreck Area
    private Vector2Int shipwreckPosition = new Vector2Int(-1, -1); //Position of the shipwreck area on the board
    public List<Vector2Int> shipwreckTiles = new List<Vector2Int>(); //List to track the tiles occupied by the shipwreck area
    public HashSet<Vector2Int> shipwreckPositions = new HashSet<Vector2Int>();
    
    [Header("Shipwreck Configurations")]
    public GameObject shipwreckCenterPrefab;
    public GameObject shipwreckSurroundingPrefab;
    private const int SHIPWRECK_GRID_SIZE = 3; //size of the shipwreck grid

    [Header("Grid Settings")]
    private const int TILE_COUNT_X = 6;
    private const int TILE_COUNT_Y = 8;
    private int[] shipwreckValidXCoordinates = { 0, 1, 2, 3 }; //valid x coordinates
    private int[] shipwreckValidYCoordinates = { 2, 3 }; //valid y coordinates

    [Header("References")]
    public Transform shipboardTransform;
    private Shipboard shipboard;

    [Header("Dependencies")]
    public ToggleCalamities toggleCalamities;
    public MersenneTwister mt;

    public List<GameObject> activeShipwreck = new List<GameObject>();
    public GameObject shipwreckCenter;

    private void Awake()
    {
        shipboard = FindObjectOfType<Shipboard>();
        // Seed the Mersenne Twister with the current time in ticks, converted to a 32-bit unsigned integer.
        uint seed = (uint)(DateTime.Now.Ticks & 0xFFFFFFFF);
        mt = new MersenneTwister(seed);
    }

    //Shipwreck Area
    private void SpawnShipwreckArea(int startX, int startY)
    {
        // Find the shipwreck center position based on the grid size.
        int centerX = startX + SHIPWRECK_GRID_SIZE / 2;
        int centerY = startY + SHIPWRECK_GRID_SIZE / 2;

        // Ensure the center position is not occupied by a ship.
        while (IsPositionOccupied(centerX, centerY))
        {
            // Find a new random position for the shipwreck using restricted coordinates.
            startX = shipwreckValidXCoordinates[mt.Next(shipwreckValidXCoordinates.Length)];
            startY = shipwreckValidYCoordinates[mt.Next(shipwreckValidYCoordinates.Length)];

            centerX = startX + SHIPWRECK_GRID_SIZE / 2;
            centerY = startY + SHIPWRECK_GRID_SIZE / 2;
        }

        shipwreckPosition = new Vector2Int(startX, startY);
        shipwreckTiles.Clear();

        for (int i = startX; i < startX + SHIPWRECK_GRID_SIZE; i++)
        {
            for (int j = startY; j < startY + SHIPWRECK_GRID_SIZE; j++)
            {
                if (i < TILE_COUNT_X && j < TILE_COUNT_Y) // Ensure within bounds
                {
                    if (i == centerX && j == centerY)
                    {

                       // Instantiate shipwreck center since it's not occupied
                       shipwreckCenter = Instantiate(shipwreckCenterPrefab, new Vector3(centerX, centerY, 0), Quaternion.identity);
                       shipwreckCenter.transform.SetParent(shipboardTransform);
                       PositionShipwreckCenter(shipwreckCenter, centerX, centerY);
                       activeShipwreck.Add(shipwreckCenter);
                       toggleCalamities.SetShipwreckCenter(shipwreckCenter);
                    }
                    else
                    {
                        // Instantiate the surrounding shipwreck prefab regardless of occupancy
                        GameObject shipwreckSurrounding = Instantiate(shipwreckSurroundingPrefab, Vector3.zero, Quaternion.identity);
                        PositionShipwreckSurrounding(shipwreckSurrounding, i, j);
                        activeShipwreck.Add(shipwreckSurrounding);
                    }
                    shipwreckPositions.Add(new Vector2Int(i, j));
                    // Add the tile to the list regardless of whether the center is occupied
                    shipwreckTiles.Add(new Vector2Int(i, j));

                    // Check if a ship is present on this tile and apply the debuff
                    ShipPieces ship = shipboard.GetShipboardPieces()[i, j];
                    if (ship != null)
                    {
                        ship.ApplyOutOfCommissionDebuff();
                    }
                }
            }
        }
    }
    // Method to spawn a random whirlpool at a valid unoccupied position.
    public void SpawnRandomShipwreckArea()
    {
        int x, y;
        do
        {
            // Pick random x and y from shipwreck-specific ranges
            x = shipwreckValidXCoordinates[mt.Next(shipwreckValidXCoordinates.Length)];
            y = shipwreckValidYCoordinates[mt.Next(shipwreckValidYCoordinates.Length)];
        }
        while (IsPositionOccupied(x + SHIPWRECK_GRID_SIZE / 2, y + SHIPWRECK_GRID_SIZE / 2)); // Ensure the center is unoccupied.

        SpawnShipwreckArea(x, y); // Spawn the shipwreck at the unoccupied position.
    }
    private bool IsPositionOccupied(int x, int y)
    {
        // Check if the position is within bounds and has a ship.
        if (x < 0 || x >= TILE_COUNT_X || y < 0 || y >= TILE_COUNT_Y)
            return true; // Consider out-of-bounds as occupied.

        return shipboard.GetShipboardPieces()[x, y] != null;
    }
    public void PositionShipwreckCenter(GameObject shipwreckCenter, int x, int y)
    {
        // Calculate the center position of the whirlpool grid cell
        Vector3 tileCenter = shipboard.TileCenter(x, y);

        // Set the position of the whirlpool center prefab
        shipwreckCenter.transform.position = tileCenter;
    }
    public void PositionShipwreckSurrounding(GameObject shipwreckSurrounding, int x, int y)
    {
        // Calculate the center position of the surrounding tile
        Vector3 tileCenter = shipboard.TileCenter(x, y);

        // Set the position of the whirlpool surrounding prefab
        shipwreckSurrounding.transform.position = tileCenter;
    }
}