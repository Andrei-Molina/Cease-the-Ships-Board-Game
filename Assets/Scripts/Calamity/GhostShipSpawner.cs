using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShipSpawner : MonoBehaviour
{
    //Shipwreck Area
    private Vector2Int ghostShipPosition = new Vector2Int(-1, -1); //Position of the shipwreck area on the board
    public List<Vector2Int> ghostShipTiles = new List<Vector2Int>(); //List to track the tiles occupied by the shipwreck area
    public HashSet<Vector2Int> ghostShipPositions = new HashSet<Vector2Int>();

    [Header("Shipwreck Configurations")]
    public GameObject ghostShipCenterPrefab;
    public GameObject ghostShipSurroundingPrefab;
    private const int GHOSTSHIP_GRID_SIZE = 3; //size of the shipwreck grid

    [Header("Grid Settings")]
    private const int TILE_COUNT_X = 6;
    private const int TILE_COUNT_Y = 8;
    private int[] ghostShipValidXCoordinates = { 0, 1, 2, 3 }; //valid x coordinates
    private int[] ghostShipValidYCoordinates = { 2, 3 }; //valid y coordinates

    [Header("References")]
    public Transform shipboardTransform;
    private Shipboard shipboard;

    [Header("Dependencies")]
    public ToggleCalamities toggleCalamities;
    public MersenneTwister mt;

    public List<GameObject> activeGhostShip = new List<GameObject>();
    public GameObject ghostShipCenter;

    private void Awake()
    {
        shipboard = FindObjectOfType<Shipboard>();
        // Seed the Mersenne Twister with the current time in ticks, converted to a 32-bit unsigned integer.
        uint seed = (uint)(DateTime.Now.Ticks & 0xFFFFFFFF);
        mt = new MersenneTwister(seed);
    }

    //Ghost Ship
    private void SpawnGhostShip(int startX, int startY)
    {
        // Find the ghost ship center position based on the grid size.
        int centerX = startX + GHOSTSHIP_GRID_SIZE / 2;
        int centerY = startY + GHOSTSHIP_GRID_SIZE / 2;

        // Ensure the center position is not occupied by a ship.
        while (IsPositionOccupied(centerX, centerY))
        {
            // Find a new random position for the shipwreck using restricted coordinates.
            startX = ghostShipValidXCoordinates[mt.Next(ghostShipValidXCoordinates.Length)];
            startY = ghostShipValidYCoordinates[mt.Next(ghostShipValidYCoordinates.Length)];

            centerX = startX + GHOSTSHIP_GRID_SIZE / 2;
            centerY = startY + GHOSTSHIP_GRID_SIZE / 2;
        }

        ghostShipPosition = new Vector2Int(startX, startY);
        ghostShipTiles.Clear();

        for (int i = startX; i < startX + GHOSTSHIP_GRID_SIZE; i++)
        {
            for (int j = startY; j < startY + GHOSTSHIP_GRID_SIZE; j++)
            {
                if (i < TILE_COUNT_X && j < TILE_COUNT_Y) // Ensure within bounds
                {
                    if (i == centerX && j == centerY)
                    {

                        // Instantiate ghost ship center since it's not occupied
                        ghostShipCenter = Instantiate(ghostShipCenterPrefab, new Vector3(centerX, centerY, 0), Quaternion.identity);
                        ghostShipCenter.transform.SetParent(shipboardTransform);
                        PositionGhostShipCenter(ghostShipCenter, centerX, centerY);
                        activeGhostShip.Add(ghostShipCenter);
                        toggleCalamities.SetShipwreckCenter(ghostShipCenter);
                    }
                    else
                    {
                        // Instantiate the surrounding shipwreck prefab regardless of occupancy
                        GameObject ghostShipSurrounding = Instantiate(ghostShipSurroundingPrefab, Vector3.zero, Quaternion.identity);
                        PositionGhostShipSurrounding(ghostShipSurrounding, i, j);
                        activeGhostShip.Add(ghostShipCenter);
                    }
                    ghostShipPositions.Add(new Vector2Int(i, j));
                    // Add the tile to the list regardless of whether the center is occupied
                    ghostShipTiles.Add(new Vector2Int(i, j));

                    // Check if a ship is present on this tile and apply the frightened debuff
                    ShipPieces ship = shipboard.GetShipboardPieces()[i, j];
                    if (ship != null)
                    {
                        ship.ApplyFrightenedDebuff("Ghost Ship");
                    }
                }
            }
        }
    }

    // Method to spawn a random whirlpool at a valid unoccupied position.
    public void SpawnRandomGhostShip()
    {
        int x, y;
        do
        {
            // Pick random x and y from shipwreck-specific ranges
            x = ghostShipValidXCoordinates[mt.Next(ghostShipValidXCoordinates.Length)];
            y = ghostShipValidYCoordinates[mt.Next(ghostShipValidYCoordinates.Length)];
        }
        while (IsPositionOccupied(x + GHOSTSHIP_GRID_SIZE / 2, y + GHOSTSHIP_GRID_SIZE / 2)); // Ensure the center is unoccupied.

        SpawnGhostShip(x, y); // Spawn the shipwreck at the unoccupied position.
    }
    private bool IsPositionOccupied(int x, int y)
    {
        // Check if the position is within bounds and has a ship.
        if (x < 0 || x >= TILE_COUNT_X || y < 0 || y >= TILE_COUNT_Y)
            return true; // Consider out-of-bounds as occupied.

        return shipboard.GetShipboardPieces()[x, y] != null;
    }
    public void PositionGhostShipCenter(GameObject ghostShipCenter, int x, int y)
    {
        // Calculate the center position of the whirlpool grid cell
        Vector3 tileCenter = shipboard.TileCenter(x, y);

        // Set the position of the whirlpool center prefab
        ghostShipCenter.transform.position = tileCenter;
    }
    public void PositionGhostShipSurrounding(GameObject ghostShipSurrounding, int x, int y)
    {
        // Calculate the center position of the surrounding tile
        Vector3 tileCenter = shipboard.TileCenter(x, y);

        // Set the position of the whirlpool surrounding prefab
        ghostShipSurrounding.transform.position = tileCenter;
    }
}
