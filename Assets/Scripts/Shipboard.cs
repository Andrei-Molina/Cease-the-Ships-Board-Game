using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Shipboard : MonoBehaviour
{
    private const int TILE_COUNT_X = 6;
    private const int TILE_COUNT_Y = 8;
    private static int player1_SP = 0;
    private static int player2_SP = 0;
    [SerializeField] private float tileSize;
    List<ShipPieces> listShip = new List<ShipPieces>();
    private ShipPieces[,] shipboardPieces;
    private Vector3 bounds;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private ShipPieces currentlyDragging;
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float yOffset = 0.35f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [Header("Prefabs")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private GameObject reefPrefab;
    [SerializeField] private GameObject dicePrefab;
    private List<ShipPieces> deadPlayer1 = new List<ShipPieces>();
    private List<ShipPieces> deadPlayer2 = new List<ShipPieces>();
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private Vector3 player1CameraLandscapePosition = new Vector3(-0.140000001f, 4.11000013f, -3.74000001f);
    private Vector3 player2CameraLandscapePosition = new Vector3(-0.140000001f, 4.11000013f, 2.6f);
    private Quaternion player1CameraLandscapeRotation = Quaternion.Euler(63.117f, 0, 0);
    private Quaternion player2CameraLandscapeRotation = Quaternion.Euler(63.117f, 180, 0);
    private Vector3 player1CameraPortraitPosition = new Vector3(-0.140000001f, 6.48999977f, -5.0999999f);
    private Vector3 player2CameraPortraitPosition = new Vector3(-0.140000001f, 6.48999977f, 4.17999983f);
    private Quaternion player1CameraPortraitRotation = Quaternion.Euler(63.117f, 0, 0);
    private Quaternion player2CameraPortraitRotation = Quaternion.Euler(63.117f, 180, 0);
    private float cameraLerpSpeed = 2f; // Speed of lerp, you can adjust this value
    //Call scripts
    public OrientationManager orientationManager;
    public MersenneTwister mt;
    public DiceRoll diceRoll;
    private bool isPlayer1Turn;

    //Tracking turn
    private bool turnEnded = false;
    private int turnCounter = 0;

    private bool isRollingDice = false;
    public Transform originalCameraPosition; // Initial camera position before transition

    // Positions and rotations for each camera transition step
    public Vector3 diceCameraPosition1 = new Vector3(-13.3f, 9.2f, 46.4f);
    public Quaternion diceCameraRotation1 = Quaternion.Euler(4.17f, 159.39f, 357.35f);

    public Vector3 diceCameraPosition2 = new Vector3(-9.6f, 9.2f, 27f);
    public Quaternion diceCameraRotation2 = Quaternion.Euler(4.17f, 159.39f, 357.35f);

    public Vector3 diceCameraPosition3 = new Vector3(-9.6f, 9.2f, 27f);
    public Quaternion diceCameraRotation3 = Quaternion.Euler(347.5f, 160.27f, 356.91f);

    public Vector3 resultViewPosition = new Vector3(-0.7f, 11.2f, 9.3f);
    public Quaternion resultViewRotation = Quaternion.Euler(49.37f, 163.69f, 358.29f);

    [SerializeField] private GameObject pirateHideoutTerrain;
    [SerializeField] private GameObject Gameboard;

    //Text
    [SerializeField] private TextMeshProUGUI tapScreenToRollDice;

    private void Start()
    {
        //Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
        isPlayer1Turn = true;
        // Seed the Mersenne Twister with the current time in ticks, converted to a 32-bit unsigned integer.
        uint seed = (uint)(DateTime.Now.Ticks & 0xFFFFFFFF);
        mt = new MersenneTwister(seed);
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
        SpawnRandomCalamity();
    }
    //Generate board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize + 0.13f, 0, (tileCountY / 2) * tileSize + 0.5f) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y: {1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        tileObject.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        tileObject.gameObject.transform.localScale = new Vector3(1.029878f, 1.06147f, 1.010923f);

        return tileObject;
    }
    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        // Only handle camera transitions if not rolling dice
        if (!isRollingDice)
        {
            HandleCameraTransition();
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")) && !isMovingInWhirlpool && !isMovingInWaterspout)
        {
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = ContainsValidMove(ref availableMoves, currentHover) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // Left click to select ship
            if (Input.GetMouseButtonDown(0))
            {
                if (shipboardPieces[hitPosition.x, hitPosition.y] != null)
                {
                    if ((shipboardPieces[hitPosition.x, hitPosition.y].team == 0 && isPlayer1Turn) || (shipboardPieces[hitPosition.x, hitPosition.y].team == 1 && !isPlayer1Turn))
                    {
                        currentlyDragging = shipboardPieces[hitPosition.x, hitPosition.y];
                        availableMoves = currentlyDragging.GetAvailableMoves(ref shipboardPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        HighlightTiles();
                    }
                }
            }

            // Release click to move ship
            if (currentlyDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);
                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);

                if (!validMove)
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));

                if (validMove)
                {
                    turnEnded = true; // Mark the turn as ended
                    turnCounter++;
                }

                // Check for Player 2's third turn
                if (turnCounter == 6 && (isPlayer1Turn && turnCounter % 6 == 0)) // 6 because each player has 3 turns
                {
                    turnCounter = 0;

                    // Use Mersenne Twister to determine if a calamity should be spawned.
                    int spawnChance = mt.Next(100); // Generate a random integer between 0 and 99.
                    ClearCalamities(); // Clear existing whirlpool and reef calamities.

                    if (spawnChance < 85) // 85% chance to spawn a calamity.
                    {
                        SpawnRandomCalamity(); // Call the method to spawn a new calamity.
                    }
                }

                currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = ContainsValidMove(ref availableMoves, currentHover) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }

        // Handle Whirlpool movement at the end of the turn
        if (turnEnded)
        {
            // Move ships in Whirlpool after the player's move
            foreach (var tile in whirlpoolTiles)
            {
                if (shipboardPieces[tile.x, tile.y] != null)
                {
                    isMovingInWhirlpool = true; // Set the flag to true
                    StartCoroutine(MoveShipsInWhirlpool()); // Move ships only once at the end of the turn
                    break; // Stop after first movement since we only need one execution per turn
                }
            }

            foreach (var tile in waterspoutTiles)
            {
                if (shipboardPieces[tile.x, tile.y] != null)
                {
                    isMovingInWaterspout = true; //Set the flag to true
                    StartCoroutine(TeleportShipsInWaterspout()); //Move ships only once at the end of the turn
                    break; //Stop after first movement since we only need one execution per turn
                }
            }

            // Move ships affected by Treacherous Current after the player's move
            foreach (var tile in treacherousCurrentPositions)
            {
                ShipPieces ship = shipboardPieces[tile.x, tile.y];
                if (ship != null) // Check if there's a ship on this tile
                {
                    // Call MoveShipWithTreacherousCurrent() to move the ship
                    MoveShipWithTreacherousCurrent(tile, ship);
                    break; // Stop after first movement since we only need one execution per turn
                }
            }

            turnEnded = false; // Reset turn ended state for the next turn
        }

        if (currentlyDragging)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance))
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * 1.0f);
        }
    }

    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;
    }

    public void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (shipboardPieces[x, y] != null)
                    PositionSinglePiece(x, y, true);
    }
    public void PositionSinglePiece(int x, int y, bool force = false)
    {
        if (shipboardPieces[x, y] != null)
        {
            shipboardPieces[x, y].currentX = x;
            shipboardPieces[x, y].currentY = y;
            shipboardPieces[x, y].SetPosition(GetTileCenter(x, y), force);
        }
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset + 0.15f, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    private void SpawnAllPieces()
    {
        shipboardPieces = new ShipPieces[TILE_COUNT_X, TILE_COUNT_Y];
        int player1 = 0;
        int player2 = 1;

        //Player 1
        shipboardPieces[2, 0] = SpawnSinglePiece(ShipPieceType.BlackFlagship, player1);
        shipboardPieces[3, 0] = SpawnSinglePiece(ShipPieceType.BlackDockyard, player1);
        shipboardPieces[0, 1] = SpawnSinglePiece(ShipPieceType.BlackDestroyerASW, player1);
        shipboardPieces[2, 1] = SpawnSinglePiece(ShipPieceType.BlackDestroyer, player1);
        shipboardPieces[3, 1] = SpawnSinglePiece(ShipPieceType.BlackDestroyer, player1);
        shipboardPieces[5, 1] = SpawnSinglePiece(ShipPieceType.BlackDestroyerASW, player1);
        shipboardPieces[0, 0] = SpawnSinglePiece(ShipPieceType.BlackSubmarine, player1);
        shipboardPieces[1, 0] = SpawnSinglePiece(ShipPieceType.BlackAircraftCarrier, player1);
        shipboardPieces[4, 0] = SpawnSinglePiece(ShipPieceType.BlackAircraftCarrier, player1);
        shipboardPieces[5, 0] = SpawnSinglePiece(ShipPieceType.BlackSubmarine, player1);
        shipboardPieces[1, 1] = SpawnSinglePiece(ShipPieceType.BlackLightCruiser, player1);
        shipboardPieces[4, 1] = SpawnSinglePiece(ShipPieceType.BlackLightCruiser, player1);

        //Player 2
        shipboardPieces[2, 7] = SpawnSinglePiece(ShipPieceType.SilverFlagship, player2);
        shipboardPieces[3, 7] = SpawnSinglePiece(ShipPieceType.SilverDockyard, player2);
        shipboardPieces[0, 6] = SpawnSinglePiece(ShipPieceType.SilverDestroyerASW, player2);
        shipboardPieces[2, 6] = SpawnSinglePiece(ShipPieceType.SilverDestroyer, player2);
        shipboardPieces[3, 6] = SpawnSinglePiece(ShipPieceType.SilverDestroyer, player2);
        shipboardPieces[5, 6] = SpawnSinglePiece(ShipPieceType.SilverDestroyerASW, player2);
        shipboardPieces[0, 7] = SpawnSinglePiece(ShipPieceType.SilverSubmarine, player2);
        shipboardPieces[1, 7] = SpawnSinglePiece(ShipPieceType.SilverAircraftCarrier, player2);
        shipboardPieces[4, 7] = SpawnSinglePiece(ShipPieceType.SilverAircraftCarrier, player2);
        shipboardPieces[5, 7] = SpawnSinglePiece(ShipPieceType.SilverSubmarine, player2);
        shipboardPieces[1, 6] = SpawnSinglePiece(ShipPieceType.SilverLightCruiser, player2);
        shipboardPieces[4, 6] = SpawnSinglePiece(ShipPieceType.SilverLightCruiser, player2);
    }
    private ShipPieces SpawnSinglePiece(ShipPieceType type, int team)
    {
        ShipPieces sp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ShipPieces>();
        sp.type = type;
        sp.team = team;

        listShip.Add(sp);

        return sp;
    }

    public bool MoveTo(ShipPieces sp, int x, int y, bool changeTurn = true)
    {
        bool isPirateHideout = pirateHideoutPositions.Contains(new Vector2Int(x, y));

        // Check if the target position is occupied by a ReefSpawner
        if (reefPositions.Contains(new Vector2Int(x, y)))
            return false;

        if (whirlpoolCenterPosition.Contains(new Vector2Int(x, y)))
            return false;

        if (waterspoutCenterPosition.Contains(new Vector2Int(x, y)))
            return false;

        if (!ContainsValidMove(ref availableMoves, new Vector2Int(x, y)))
            return false;

        Vector2Int previousPosition = new Vector2Int(sp.currentX, sp.currentY);

        //Is there another piece on the target position?
        if (shipboardPieces[x, y] != null)
        {
            Debug.Log("Dice is not yet instantiated");
            ShipPieces osp = shipboardPieces[x, y];

            if (sp.team == osp.team)
                return false;

            //If it's the enemy team
            if (osp.team == 0)
            {
                deadPlayer1.Add(osp);
                osp.gameObject.SetActive(false);
            }

            else
            {
                deadPlayer2.Add(osp);
                osp.gameObject.SetActive(false);
            }
        }

        shipboardPieces[x, y] = sp;
        shipboardPieces[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x, y);

        if (isPirateHideout && !isRollingDice)
        {
            StartCoroutine(MoveCameraAndRollDice());
        }

        if (changeTurn)
        {
            isPlayer1Turn = !isPlayer1Turn;
        }

        return true;
    }

    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            Vector2Int tilePosition = new Vector2Int(availableMoves[i].x, availableMoves[i].y);

            // Check if the position is restricted by a reef, whirlpool, or waterspout
            if (reefPositions.Contains(tilePosition) || whirlpoolCenterPosition.Contains(tilePosition) || waterspoutCenterPosition.Contains(tilePosition))
            {
                continue; // Skip this tile if it contains a reef, whirlpool, or waterspout
            }

            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }
    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }

        availableMoves.Clear();
    }
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2Int pos)
    {
        for (int i = 0; i < moves.Count; i++)
            if (moves[i].x == pos.x && moves[i].y == pos.y)
                return true;

        return false;
    }
    public ShipPieces[,] GetShipboardPieces()
    {
        return shipboardPieces;
    }

    private void HandleCameraTransition()
    {
        if (isRollingDice) return; // Skip handling camera transition if a dice roll is in progress

        // Camera transition based on the current player
        if (isPlayer1Turn && orientationManager.IsLandscape)
        {
            currentCamera.transform.position = Vector3.Lerp(currentCamera.transform.position, player1CameraLandscapePosition, Time.deltaTime * cameraLerpSpeed);
            currentCamera.transform.rotation = Quaternion.Lerp(currentCamera.transform.rotation, player1CameraLandscapeRotation, Time.deltaTime * cameraLerpSpeed);
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        else if (!isPlayer1Turn && orientationManager.IsLandscape)
        {
            currentCamera.transform.position = Vector3.Lerp(currentCamera.transform.position, player2CameraLandscapePosition, Time.deltaTime * cameraLerpSpeed);
            currentCamera.transform.rotation = Quaternion.Lerp(currentCamera.transform.rotation, player2CameraLandscapeRotation, Time.deltaTime * cameraLerpSpeed);
            Screen.orientation = ScreenOrientation.LandscapeRight;
        }
        else if (isPlayer1Turn && !orientationManager.IsLandscape)
        {
            currentCamera.transform.position = Vector3.Lerp(currentCamera.transform.position, player1CameraPortraitPosition, Time.deltaTime * cameraLerpSpeed);
            currentCamera.transform.rotation = Quaternion.Lerp(currentCamera.transform.rotation, player1CameraPortraitRotation, Time.deltaTime * cameraLerpSpeed);
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else if (!isPlayer1Turn && !orientationManager.IsLandscape)
        {
            currentCamera.transform.position = Vector3.Lerp(currentCamera.transform.position, player2CameraPortraitPosition, Time.deltaTime * cameraLerpSpeed);
            currentCamera.transform.rotation = Quaternion.Lerp(currentCamera.transform.rotation, player2CameraPortraitRotation, Time.deltaTime * cameraLerpSpeed);
            Screen.orientation = ScreenOrientation.PortraitUpsideDown;
        }
    }

    private void CheckDiceRoll()
    {
        StartCoroutine(CheckRollStatus());
    }

    private IEnumerator CheckRollStatus()
    {
        // Wait for a brief moment to ensure the dice is settled
        yield return new WaitForSeconds(1f);

        // Check if the dice has stopped rolling
        if (diceRoll != null && diceRoll.GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            // Make sure the dice face has been detected
            if (diceRoll.diceFaceNum > 0)
            {
                int diceFace = diceRoll.diceFaceNum;
                Debug.Log("Final Dice Face: " + diceFace);

                AwardSkillPoints(diceFace);
                ClearCalamities();

                // Move camera to the result view position after dice lands
                StartCoroutine(MoveToResultViewAndReturn());
            }
            else
            {
                Debug.LogWarning("Dice face has not been detected yet!");
            }
        }
    }

    private IEnumerator MoveCameraAndRollDice()
    {
        Gameboard.gameObject.SetActive(false);
        pirateHideoutTerrain.gameObject.SetActive(true);
        isRollingDice = true;

        // Move to position 1
        yield return StartCoroutine(LerpCameraToPosition(diceCameraPosition1, diceCameraRotation1));

        yield return new WaitForSeconds(0.1f);

        // Move to position 2
        yield return StartCoroutine(LerpCameraToPosition(diceCameraPosition2, diceCameraRotation2));

        // Wait for player tap to move to position 3 and start dice roll
        yield return StartCoroutine(WaitForPlayerTap());

        // Move to position 3 after tap
        yield return StartCoroutine(LerpCameraToPosition(diceCameraPosition3, diceCameraRotation3));

        // Instantiate the dice and start the roll
        GameObject diceObject = Instantiate(dicePrefab, new Vector3(-2.2f, 20.1f, 6.4f), Quaternion.identity);
        Debug.Log("Dice is instantiated");

        // Get the DiceRoll component from the instantiated object
        diceRoll = diceObject.GetComponent<DiceRoll>();
        diceRoll.RollDice(); // Start the dice roll

        Debug.Log("Rolling dice...");

        yield return new WaitForSeconds(0.5f); // Wait time to allow the dice to gain velocity

        // Wait until the dice has stopped rolling
        yield return new WaitUntil(() => diceRoll != null && diceRoll.GetComponent<Rigidbody>().velocity == Vector3.zero);

        // Perform dice roll check
        CheckDiceRoll();

        yield return new WaitForSeconds(5f);

        Destroy(diceRoll.gameObject);
        diceRoll = null;
        pirateHideoutTerrain.gameObject.SetActive(false);
        Gameboard.gameObject.SetActive(true);
    }

    private IEnumerator WaitForPlayerTap()
    {
        Debug.Log("Waiting for player tap to roll the dice...");
        Color textColor = tapScreenToRollDice.color;
        float duration = 1f; // Duration for the fade effect
        float elapsedTime = 0f;

        while (!Input.GetMouseButtonDown(0))
        {
            // Fade in and out
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.PingPong(elapsedTime / duration, 1f); // Value between 0 and 1
            textColor.a = alpha;
            tapScreenToRollDice.color = textColor;

            // Wait until the player taps/clicks on the screen
            tapScreenToRollDice.gameObject.SetActive(true);
            yield return null;
        }

        textColor.a = 1f; // Ensure it's fully opaque after tap
        tapScreenToRollDice.color = textColor;
        tapScreenToRollDice.gameObject.SetActive(false);
        Debug.Log("Player tapped! Initiating dice roll...");
    }

    private IEnumerator MoveToResultViewAndReturn()
    {
        // Start moving the dice at the same time as the camera
        Vector3 diceTargetPosition = new Vector3(-1.96046424f, 7.59984255f, 6.40341902f);
        float moveDuration = 0f; // Adjust the duration as needed

        // Start moving the dice and camera at the same time
        StartCoroutine(MoveDiceToPosition(diceTargetPosition, moveDuration));

        // Lerp camera to the result view position after dice lands
        yield return StartCoroutine(LerpCameraToPosition(resultViewPosition, resultViewRotation));

        // Wait for 3 seconds before returning to original position
        yield return new WaitForSeconds(3f);

        // Return to original camera position
        yield return StartCoroutine(ReturnCameraToOriginalPosition());
    }

    private IEnumerator ReturnCameraToOriginalPosition()
    {
        yield return StartCoroutine(LerpCameraToPosition(originalCameraPosition.position, originalCameraPosition.rotation));
        // Reset the dice roll variables
        isRollingDice = false; // Resume normal camera transitions
    }

    private IEnumerator LerpCameraToPosition(Vector3 targetPosition, Quaternion targetRotation)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = currentCamera.transform.position;
        Quaternion startingRot = currentCamera.transform.rotation;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * cameraLerpSpeed;
            currentCamera.transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime);
            currentCamera.transform.rotation = Quaternion.Lerp(startingRot, targetRotation, elapsedTime);
            yield return null; // Wait for the next frame
        }

        // Ensure the camera reaches the exact target position and rotation
        currentCamera.transform.position = targetPosition;
        currentCamera.transform.rotation = targetRotation;
    }

    private IEnumerator MoveDiceToPosition(Vector3 targetPosition, float duration)
    {
        // Get the starting position of the dice
        Vector3 startPosition = diceRoll.transform.position;

        // Track time to perform the lerp over the specified duration
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Lerp the dice's position
            diceRoll.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            // Wait for the next frame before continuing the loop
            yield return null;
        }

        // Ensure the final position is exactly the target position after lerp completes
        diceRoll.transform.position = targetPosition;
    }

    //Testing Zone

    //Calamities
    //initialize all needed variables
    //whirlpool
    private Vector2Int whirlpoolPosition = new Vector2Int(-1, -1); // Position of the whirlpool on the board
    private List<Vector2Int> whirlpoolTiles = new List<Vector2Int>(); // List to track the tiles occupied by the whirlpool
    [SerializeField] private GameObject whirlpoolCenterPrefab;
    [SerializeField] private GameObject whirlpoolSurroundingPrefab;
    public GameObject whirlpoolCenter;
    // Define the valid ranges for x and y coordinates
    private int[] whirlpoolValidXCoordinates = { 0, 1, 2, 3 }; // Whirlpool valid x-coordinates
    private int[] whirlpoolValidYCoordinates = { 2, 3 };       // Whirlpool valid y-coordinates
    private HashSet<Vector2Int> whirlpoolCenterPosition = new HashSet<Vector2Int>();
    private const int WHIRLPOOL_GRID_SIZE = 3; // Size of the Waterspout grid

    //Reef
    private HashSet<Vector2Int> reefPositions = new HashSet<Vector2Int>();
    // Define the valid ranges for x and y coordinates
    private int[] reefValidXCoordinates = { 0, 1, 2, 3, 4, 5 }; // Reef valid x-coordinates
    private int reefMinY = 2; // Reef minimum y-coordinate
    private int reefMaxY = 5; // Reef maximum y-coordinate

    //Treacherous Current
    private HashSet<Vector2Int> treacherousCurrentPositions = new HashSet<Vector2Int>();
    // Treacherous Current prefabs for different directions
    [SerializeField] private GameObject[] treacherousCurrentPrefabs; // Array to hold prefabs for Left, Up, Right, Down
    private Quaternion[] treacherousCurrentRotations = {
    Quaternion.Euler(0, 0, 0),      // TreacherousCurrent-Left
    Quaternion.Euler(0, 90, 0),     // TreacherousCurrent-Up
    Quaternion.Euler(0, 180, 0),    // TreacherousCurrent-Right
    Quaternion.Euler(0, 270, 0)      // TreacherousCurrent-Down
};
    // Define the valid ranges for x and y coordinates
    private int[] treacherousCurrentValidXCoordinates = { 0, 1, 2, 3, 4, 5 }; // Reef valid x-coordinates
    private int treacherousCurrentMinY = 2; // Reef minimum y-coordinate
    private int treacherousCurrentMaxY = 5; // Reef maximum y-coordinate

    //Pirate Hideout
    private HashSet<Vector2Int> pirateHideoutPositions = new HashSet<Vector2Int>();
    [SerializeField] private GameObject pirateHideoutPrefab;
    // Define the valid ranges for x and y coordinates
    private int[] pirateHideoutValidXCoordinates = { 0, 1, 2, 3, 4, 5 }; // Reef valid x-coordinates
    private int pirateHideoutMinY = 2; // Reef minimum y-coordinate
    private int pirateHideoutMaxY = 5; // Reef maximum y-coordinate

    //Waterspout
    private Vector2Int waterspoutPosition = new Vector2Int(-1, -1); // Position of the whirlpool on the board
    private List<Vector2Int> waterspoutTiles = new List<Vector2Int>(); // List to track the tiles occupied by the whirlpool
    [SerializeField] private GameObject waterspoutCenterPrefab;
    [SerializeField] private GameObject waterspoutSurroundingPrefab;
    public GameObject waterspoutCenter;
    private int[] waterspoutValidXCoordinates = { 0, 1, 2, 3 };
    private int[] waterspoutValidYCoordinates = { 2, 3 };
    private HashSet<Vector2Int> waterspoutCenterPosition = new HashSet<Vector2Int>();
    private const int WATERSPOUT_GRID_SIZE = 3; // Size of the Waterspout grid

    //Calamities
    private bool isMovingInWhirlpool = false; // Add this line
    private bool isMovingInWaterspout = false;

    private List<GameObject> activeWhirlpools = new List<GameObject>();
    private List<GameObject> activeWaterspouts = new List<GameObject>();
    private List<GameObject> activeReefs = new List<GameObject>();
    private List<GameObject> activeTreacherousCurrents = new List<GameObject>();
    private List<GameObject> activePirateHideouts = new List<GameObject>();


    //Whirlpool
    private void SpawnWhirlpool(int startX, int startY)
    {
        // Find the whirlpool center position based on the grid size.
        int centerX = startX + WHIRLPOOL_GRID_SIZE / 2;
        int centerY = startY + WHIRLPOOL_GRID_SIZE / 2;

        // Ensure the center position is not occupied by a ship.
        while (IsPositionOccupied(centerX, centerY))
        {
            // Find a new random position for the whirlpool using restricted coordinates.
            startX = whirlpoolValidXCoordinates[mt.Next(whirlpoolValidXCoordinates.Length)];
            startY = whirlpoolValidYCoordinates[mt.Next(whirlpoolValidYCoordinates.Length)];

            centerX = startX + WHIRLPOOL_GRID_SIZE / 2;
            centerY = startY + WHIRLPOOL_GRID_SIZE / 2;
        }

        whirlpoolPosition = new Vector2Int(startX, startY);
        whirlpoolTiles.Clear();

        for (int i = startX; i < startX + WHIRLPOOL_GRID_SIZE; i++)
        {
            for (int j = startY; j < startY + WHIRLPOOL_GRID_SIZE; j++)
            {
                if (i < TILE_COUNT_X && j < TILE_COUNT_Y) // Ensure within bounds
                {
                    // Check if the current tile is occupied before adding.
                    // Only check occupation for the center whirlpool
                    if (i == centerX && j == centerY)
                    {
                        // Check if the center position is occupied
                        if (!IsPositionOccupied(i, j))
                        {
                            // Instantiate whirlpool center since it's not occupied
                            whirlpoolCenter = Instantiate(whirlpoolCenterPrefab, new Vector3(centerX, centerY, 0), Quaternion.identity);
                            whirlpoolCenter.transform.SetParent(transform);
                            PositionWhirlpoolCenter(whirlpoolCenter, centerX, centerY);
                            whirlpoolCenterPosition.Add(new Vector2Int(centerX, centerY));
                            activeWhirlpools.Add(whirlpoolCenter);
                        }
                    }
                    else
                    {
                        // Instantiate the surrounding whirlpool prefab regardless of occupancy
                        GameObject whirlpoolSurrounding = Instantiate(whirlpoolSurroundingPrefab, Vector3.zero, Quaternion.identity);
                        PositionWhirlpoolSurrounding(whirlpoolSurrounding, i, j);
                        activeWhirlpools.Add(whirlpoolSurrounding);
                    }

                    // Add the tile to the list regardless of whether the center is occupied
                    whirlpoolTiles.Add(new Vector2Int(i, j));
                }
            }
        }
    }
    // Method to spawn a random whirlpool at a valid unoccupied position.
    private void SpawnRandomWhirlpool()
    {
        int x, y;
        do
        {
            // Pick random x and y from whirlpool-specific ranges
            x = whirlpoolValidXCoordinates[mt.Next(whirlpoolValidXCoordinates.Length)];
            y = whirlpoolValidYCoordinates[mt.Next(whirlpoolValidYCoordinates.Length)];
        }
        while (IsPositionOccupied(x + WHIRLPOOL_GRID_SIZE / 2, y + WHIRLPOOL_GRID_SIZE / 2)); // Ensure the center is unoccupied.

        SpawnWhirlpool(x, y); // Spawn the whirlpool at the unoccupied position.
    }
    private IEnumerator MoveShipsInWhirlpool()
    {
        yield return new WaitForSeconds(0.5f);

        if (whirlpoolPosition == new Vector2Int(-1, -1))
            yield break;

        // Determine the order of the tiles in the 3x3 grid in a clockwise manner
        List<Vector2Int> orderedTiles = new List<Vector2Int>
    {
        new Vector2Int(whirlpoolPosition.x, whirlpoolPosition.y),
        new Vector2Int(whirlpoolPosition.x, whirlpoolPosition.y + 1),
        new Vector2Int(whirlpoolPosition.x, whirlpoolPosition.y + 2),
        new Vector2Int(whirlpoolPosition.x + 1, whirlpoolPosition.y + 2),
        new Vector2Int(whirlpoolPosition.x + 2, whirlpoolPosition.y + 2),
        new Vector2Int(whirlpoolPosition.x + 2, whirlpoolPosition.y + 1),
        new Vector2Int(whirlpoolPosition.x + 2, whirlpoolPosition.y),
        new Vector2Int(whirlpoolPosition.x + 1, whirlpoolPosition.y)
    };

        // Store the ships (or nulls) currently in the waterspout tiles
        ShipPieces[] shipsInWaterspout = new ShipPieces[orderedTiles.Count];
        for (int i = 0; i < orderedTiles.Count; i++)
        {
            Vector2Int currentTile = orderedTiles[i];
            shipsInWaterspout[i] = shipboardPieces[currentTile.x, currentTile.y];
        }

        // Prepare a temporary array to store the new positions of the ships
        ShipPieces[] newShipPositions = new ShipPieces[orderedTiles.Count];

        // First pass: move each ship to the next tile in the clockwise order
        for (int i = 0; i < orderedTiles.Count; i++)
        {
            Vector2Int nextTile = orderedTiles[(i + 1) % orderedTiles.Count];
            ShipPieces ship = shipsInWaterspout[i];

            // Move ship to next position in temporary array (but do not update the board yet)
            if (ship != null)
            {
                newShipPositions[(i + 1) % orderedTiles.Count] = ship;
            }
        }

        // Second pass: update the actual board with the new positions
        for (int i = 0; i < orderedTiles.Count; i++)
        {
            Vector2Int currentTile = orderedTiles[i];
            ShipPieces ship = newShipPositions[i];

            // Update board with new ship positions
            shipboardPieces[currentTile.x, currentTile.y] = ship;

            // If there's a ship, update its visual position
            if (ship != null)
            {
                PositionSinglePiece(currentTile.x, currentTile.y);
            }
        }

        // After all ships are moved, handle conflicts (e.g., "eating" enemy ships)
        for (int i = 0; i < orderedTiles.Count; i++)
        {
            Vector2Int currentTile = orderedTiles[i];
            ShipPieces ship = shipboardPieces[currentTile.x, currentTile.y];

            if (ship != null)
            {
                // Check for conflicts only if two ships occupy the same space
                ShipPieces targetShip = shipboardPieces[currentTile.x, currentTile.y];

                if (targetShip != null && ship != targetShip && ship.team != targetShip.team)
                {
                    // Remove the enemy ship based on its team
                    if (targetShip.team == 0)
                    {
                        deadPlayer1.Add(targetShip);
                    }
                    else
                    {
                        deadPlayer2.Add(targetShip);
                    }

                    targetShip.gameObject.SetActive(false); // Deactivate the target ship
                    shipboardPieces[currentTile.x, currentTile.y] = ship; // Keep the moving ship
                }
            }
        }

        isMovingInWhirlpool = false; // Set the flag to false
    }
    public void PositionWhirlpoolCenter(GameObject whirlpoolCenter, int x, int y)
    {
        // Calculate the center position of the whirlpool grid cell
        Vector3 tileCenter = GetTileCenter(x, y);

        // Set the position of the whirlpool center prefab
        whirlpoolCenter.transform.position = tileCenter;
    }
    public void PositionWhirlpoolSurrounding(GameObject whirlpoolSurrounding, int x, int y)
    {
        // Calculate the center position of the surrounding tile
        Vector3 tileCenter = GetTileCenter(x, y);

        // Set the position of the whirlpool surrounding prefab
        whirlpoolSurrounding.transform.position = tileCenter;
    }
    //Reef
    private void PlaceReef(int x, int y)
    {
        reefPositions.Add(new Vector2Int(x, y));
    }
    private void SpawnRandomReef()
    {
        // Find a random position for the reef that is not occupied.
        int x, y;
        do
        {
            x = reefValidXCoordinates[mt.Next(reefValidXCoordinates.Length)];
            y = mt.Next(reefMaxY - reefMinY + 1) + reefMinY;
        }
        while (IsPositionOccupied(x, y)); // Ensure the position is not occupied.

        SpawnReef(x, y); // Spawn the reef at the unoccupied position.
    }
    private void SpawnReef(int x, int y)
    {
        // Ensure the reef position is not occupied by a ship.
        while (IsPositionOccupied(x, y))
        {
            // Find a new random position for the reef.
            x = reefValidXCoordinates[mt.Next(reefValidXCoordinates.Length)];
            y = mt.Next(reefMaxY - reefMinY + 1) + reefMinY;
        }

        // Instantiate the ReefSpawner at the specified position.
        GameObject reef = Instantiate(reefPrefab, new Vector3(x, 0, y), Quaternion.identity);
        reef.transform.SetParent(transform);

        // Position the ReefSpawner correctly in the grid.
        PositionReef(reef, x, y);

        // Track the instantiated reef GameObject.
        activeReefs.Add(reef);

        // Add the position to the reef positions set.
        PlaceReef(x, y);
    }
    public void PositionReef(GameObject reef, int x, int y, bool force = false)
    {
        // Calculate the center position of the grid cell
        Vector3 tileCenter = GetTileCenter(x, y);

        // Set the position of the ReefSpawner prefab
        reef.transform.position = tileCenter;
    }
    //Waterspout
    //Spawn waterspout
    private void SpawnWaterSpout(int startX, int startY)
    {
        // Find the water spout center position based on the grid size.
        int centerX = startX + WATERSPOUT_GRID_SIZE / 2;
        int centerY = startY + WATERSPOUT_GRID_SIZE / 2;

        // Ensure the center position is not occupied by a ship.
        while (IsPositionOccupied(centerX, centerY))
        {
            // Find a new random position for the water spout using restricted coordinates.
            startX = waterspoutValidXCoordinates[mt.Next(waterspoutValidXCoordinates.Length)];
            startY = waterspoutValidYCoordinates[mt.Next(waterspoutValidYCoordinates.Length)];

            centerX = startX + WATERSPOUT_GRID_SIZE / 2;
            centerY = startY + WATERSPOUT_GRID_SIZE / 2;
        }

        // Set the position of the water spout
        waterspoutPosition = new Vector2Int(startX, startY);
        waterspoutTiles.Clear();

        // Loop through the 3x3 grid to instantiate the water spout elements.
        for (int i = startX; i < startX + WATERSPOUT_GRID_SIZE; i++)
        {
            for (int j = startY; j < startY + WATERSPOUT_GRID_SIZE; j++)
            {
                // Ensure within bounds
                if (i < TILE_COUNT_X && j < TILE_COUNT_Y)
                {
                    // Check if the tile is the center
                    if (i == centerX && j == centerY)
                    {
                        // If the center is unoccupied, instantiate the water spout center.
                        if (!IsPositionOccupied(i, j))
                        {
                            waterspoutCenter = Instantiate(waterspoutCenterPrefab, new Vector3(centerX, centerY, 0), Quaternion.identity);
                            waterspoutCenter.transform.SetParent(transform);
                            PositionWaterspoutCenter(waterspoutCenter, centerX, centerY);
                            waterspoutCenterPosition.Add(new Vector2Int(centerX, centerY));
                            activeWaterspouts.Add(waterspoutCenter);
                        }
                    }
                    else
                    {
                        // Instantiate the surrounding tiles of the water spout.
                        GameObject waterspoutSurrounding = Instantiate(waterspoutSurroundingPrefab, Vector3.zero, Quaternion.identity);
                        PositionWaterspoutSurrounding(waterspoutSurrounding, i, j);
                        activeWaterspouts.Add(waterspoutSurrounding);
                    }

                    // Add the tile to the list of water spout tiles.
                    waterspoutTiles.Add(new Vector2Int(i, j));
                }
            }
        }
    }
    // Method to spawn a random waterspout at a valid unoccupied position.
    private void SpawnRandomWaterspout()
    {
        int x, y;
        do
        {
            // Pick random x and y from whirlpool-specific ranges
            x = waterspoutValidXCoordinates[mt.Next(waterspoutValidXCoordinates.Length)];
            y = waterspoutValidYCoordinates[mt.Next(waterspoutValidYCoordinates.Length)];
        }
        while (IsPositionOccupied(x + WATERSPOUT_GRID_SIZE / 2, y + WATERSPOUT_GRID_SIZE / 2)); // Ensure the center is unoccupied.

        SpawnWaterSpout(x, y); // Spawn the whirlpool at the unoccupied position.
    }
    private IEnumerator TeleportShipsInWaterspout()
    {
        yield return new WaitForSeconds(0.5f);

        if (waterspoutPosition == new Vector2Int(-1, -1))
            yield break;

        // Define the 3x3 grid positions, excluding the center
        List<Vector2Int> waterspoutTiles = new List<Vector2Int>
    {
        new Vector2Int(waterspoutPosition.x, waterspoutPosition.y),
        new Vector2Int(waterspoutPosition.x, waterspoutPosition.y + 1),
        new Vector2Int(waterspoutPosition.x, waterspoutPosition.y + 2),
        new Vector2Int(waterspoutPosition.x + 1, waterspoutPosition.y + 2),
        new Vector2Int(waterspoutPosition.x + 2, waterspoutPosition.y + 2),
        new Vector2Int(waterspoutPosition.x + 2, waterspoutPosition.y + 1),
        new Vector2Int(waterspoutPosition.x + 2, waterspoutPosition.y),
        new Vector2Int(waterspoutPosition.x + 1, waterspoutPosition.y)
    };

        // Store ships (or nulls) currently in the waterspout tiles
        ShipPieces[] shipsInWaterspout = new ShipPieces[waterspoutTiles.Count];
        for (int i = 0; i < waterspoutTiles.Count; i++)
        {
            Vector2Int currentTile = waterspoutTiles[i];
            shipsInWaterspout[i] = shipboardPieces[currentTile.x, currentTile.y];
        }

        // List of available positions (all tiles except occupied ones)
        List<Vector2Int> availablePositions = new List<Vector2Int>(waterspoutTiles);

        // Remove positions that are already occupied by ships
        foreach (ShipPieces ship in shipsInWaterspout)
        {
            if (ship != null)
            {
                Vector2Int occupiedPosition = new Vector2Int(ship.currentX, ship.currentY);
                availablePositions.Remove(occupiedPosition); // Make sure the ship's current position is not available
            }
        }

        // Randomly teleport ships to available positions
        System.Random random = new System.Random();
        foreach (ShipPieces ship in shipsInWaterspout)
        {
            if (ship != null)
            {
                // Get a random unoccupied position
                if (availablePositions.Count > 0)
                {
                    int randomIndex = random.Next(availablePositions.Count);
                    Vector2Int newTile = availablePositions[randomIndex];

                    // Move the ship to the new tile in the game logic
                    shipboardPieces[ship.currentX, ship.currentY] = null; // Remove from old position
                    shipboardPieces[newTile.x, newTile.y] = ship;         // Place in new position

                    // Visually update ship's position
                    PositionSinglePiece(newTile.x, newTile.y);

                    // Remove this position from available list to avoid double teleportation
                    availablePositions.RemoveAt(randomIndex);
                }
            }
        }

        // After teleportation, handle any ship conflicts (if necessary)
        for (int i = 0; i < waterspoutTiles.Count; i++)
        {
            Vector2Int currentTile = waterspoutTiles[i];
            ShipPieces ship = shipboardPieces[currentTile.x, currentTile.y];

            if (ship != null)
            {
                // Handle conflicts such as "eating" enemy ships (same as original logic)
                ShipPieces targetShip = shipboardPieces[currentTile.x, currentTile.y];

                if (targetShip != null && ship != targetShip && ship.team != targetShip.team)
                {
                    // Remove the enemy ship based on its team
                    if (targetShip.team == 0)
                    {
                        deadPlayer1.Add(targetShip);
                    }
                    else
                    {
                        deadPlayer2.Add(targetShip);
                    }

                    targetShip.gameObject.SetActive(false); // Deactivate the target ship
                    shipboardPieces[currentTile.x, currentTile.y] = ship; // Keep the moving ship
                }
            }
        }

        isMovingInWaterspout = false; // Reset flag
    }
    public void PositionWaterspoutCenter(GameObject waterspoutCenter, int x, int y)
    {
        // Calculate the center position of the whirlpool grid cell
        Vector3 tileCenter = GetTileCenter(x, y);

        // Set the position of the whirlpool center prefab
        waterspoutCenter.transform.position = tileCenter;
    }
    public void PositionWaterspoutSurrounding(GameObject waterspoutSurrounding, int x, int y)
    {
        // Calculate the center position of the surrounding tile
        Vector3 tileCenter = GetTileCenter(x, y);

        // Set the position of the whirlpool surrounding prefab
        waterspoutSurrounding.transform.position = tileCenter;
    }
    //Treacherous Current

    // Method to spawn a Treacherous Current with a random rotation at a specified position.
    //Treacherous Current position to prefab index mapping
    private Dictionary<Vector2Int, int> treacherousCurrentPositionToPrefabIndex = new Dictionary<Vector2Int, int>();

    private void SpawnTreacherousCurrent(int x, int y)
    {
        // Ensure the Treacherous Current position is not occupied by a ship or another Treacherous Current.
        while (IsPositionOccupied(x, y) || treacherousCurrentPositions.Contains(new Vector2Int(x, y)))
        {
            // Find a new random position for the Treacherous Current.
            x = treacherousCurrentValidXCoordinates[mt.Next(treacherousCurrentValidXCoordinates.Length)];
            y = mt.Next(treacherousCurrentMaxY - treacherousCurrentMinY + 1) + treacherousCurrentMinY;
        }

        // Randomly select one of the four Treacherous Current prefabs
        int prefabIndex = mt.Next(treacherousCurrentPrefabs.Length);
        GameObject selectedPrefab = treacherousCurrentPrefabs[prefabIndex];
        Quaternion selectedRotation = treacherousCurrentRotations[prefabIndex];

        // Instantiate the selected Treacherous Current prefab at the specified position with the correct rotation.
        GameObject treacherousCurrent = Instantiate(selectedPrefab, new Vector3(x, 0, y), selectedRotation);
        treacherousCurrent.transform.SetParent(transform);

        // Position the Treacherous Current correctly in the grid.
        PositionTreacherousCurrent(treacherousCurrent, x, y);

        // Track the instantiated Treacherous Current GameObject.
        activeTreacherousCurrents.Add(treacherousCurrent);

        // Add the position to the Treacherous Current positions set.
        Vector2Int currentPosition = new Vector2Int(x, y);
        treacherousCurrentPositions.Add(currentPosition);

        // Map the position to the prefab index for movement tracking
        treacherousCurrentPositionToPrefabIndex[currentPosition] = prefabIndex;
    }
    private void SpawnRandomTreacherousCurrent()
    {
        // Find a random position for the Treacherous Current that is not occupied.
        int x, y;
        do
        {
            // Select a random x-coordinate from the valid x-coordinate array.
            x = treacherousCurrentValidXCoordinates[mt.Next(treacherousCurrentValidXCoordinates.Length)];

            // Select a random y-coordinate within the defined valid range.
            y = mt.Next(treacherousCurrentMaxY - treacherousCurrentMinY + 1) + treacherousCurrentMinY;
        }
        // Ensure the position is not already occupied by another object (e.g., a ship, reef, or existing Treacherous Current).
        while (IsPositionOccupied(x, y) || treacherousCurrentPositions.Contains(new Vector2Int(x, y)));

        // Spawn the Treacherous Current at the unoccupied position.
        SpawnTreacherousCurrent(x, y);
    }

    // Method to position the Treacherous Current correctly on the grid.
    public void PositionTreacherousCurrent(GameObject treacherousCurrent, int x, int y, bool force = false)
    {
        // Calculate the center position of the grid cell
        Vector3 tileCenter = GetTileCenter(x, y);

        // Set the position of the Treacherous Current prefab
        treacherousCurrent.transform.position = tileCenter;
    }
    private void MoveShipWithTreacherousCurrent(Vector2Int treacherousCurrentPosition, ShipPieces ship)
    {
        // Check if the Treacherous Current position is in the dictionary and get the prefab index.
        if (!treacherousCurrentPositionToPrefabIndex.TryGetValue(treacherousCurrentPosition, out int prefabIndex))
        {
            // If no corresponding prefab found, return early (this shouldn't happen in normal conditions)
            Debug.LogError("No Treacherous Current found at the specified position.");
            return;
        }

        // Retrieve the rotation of the spawned prefab to determine movement direction
        Quaternion treacherousCurrentRotation = treacherousCurrentRotations[prefabIndex];
        Vector2Int movementDirection = GetMovementDirection(treacherousCurrentRotation);

        Vector2Int newPosition = treacherousCurrentPosition + movementDirection;

        // Check if the new position is within bounds
        if (newPosition.x >= 0 && newPosition.x < shipboardPieces.GetLength(0) &&
            newPosition.y >= 0 && newPosition.y < shipboardPieces.GetLength(1))
        {
            ShipPieces targetShip = shipboardPieces[newPosition.x, newPosition.y];

            // Move the ship only if the target position is empty or occupied by an enemy ship
            if (targetShip == null || targetShip.team != ship.team)
            {
                // Handle conflicts (e.g., capturing enemy ship)
                if (targetShip != null && targetShip.team != ship.team)
                {
                    // Capture the enemy ship
                    CaptureShip(targetShip);
                }

                // Move the ship to the new position
                shipboardPieces[treacherousCurrentPosition.x, treacherousCurrentPosition.y] = null; // Remove from current position
                shipboardPieces[newPosition.x, newPosition.y] = ship; // Place in new position

                // Update the ship's visual position
                PositionSinglePiece(newPosition.x, newPosition.y);
            }
        }
    }

    private Vector2Int GetMovementDirection(Quaternion rotation)
    {
        // Correct mapping of movement directions based on rotation:
        if (rotation == Quaternion.Euler(0, 0, 0))
            return new Vector2Int(-1, 0); // Move leftward
        if (rotation == Quaternion.Euler(0, 90, 0))
            return new Vector2Int(0, 1); // Move upward
        if (rotation == Quaternion.Euler(0, 180, 0))
            return new Vector2Int(1, 0); // Move rightward
        if (rotation == Quaternion.Euler(0, 270, 0))
            return new Vector2Int(0, -1); // Move downward
        return Vector2Int.zero;
    }

    private void CaptureShip(ShipPieces targetShip)
    {
        // Remove the enemy ship based on its team
        if (targetShip.team == 0)
        {
            deadPlayer1.Add(targetShip);
        }
        else
        {
            deadPlayer2.Add(targetShip);
        }

        // Deactivate or destroy the target ship
        targetShip.gameObject.SetActive(false);

        // Remove from the board array
        Vector2Int position = new Vector2Int(targetShip.currentX, targetShip.currentY);
        shipboardPieces[position.x, position.y] = null;
    }

    //Pirate Hideout
    private void PlacePirateHideout(int x, int y)
    {
        pirateHideoutPositions.Add(new Vector2Int(x, y));
    }

    private void SpawnRandomPirateHideout()
    {
        // Find a random position for the pirate hideout that is not occupied.
        int x, y;
        do
        {
            x = pirateHideoutValidXCoordinates[mt.Next(pirateHideoutValidXCoordinates.Length)];
            y = mt.Next(pirateHideoutMaxY - pirateHideoutMinY + 1) + pirateHideoutMinY;
        }
        while (IsPositionOccupied(x, y)); // Ensure the position is not occupied.

        SpawnPirateHideout(x, y); // Spawn the pirate hideout at the unoccupied position.
    }

    private void SpawnPirateHideout(int x, int y)
    {
        // Ensure the pirate hideout position is not occupied by a ship.
        while (IsPositionOccupied(x, y))
        {
            // Find a new random position for the pirate hideout.
            x = pirateHideoutValidXCoordinates[mt.Next(pirateHideoutValidXCoordinates.Length)];
            y = mt.Next(pirateHideoutMaxY - pirateHideoutMinY + 1) + pirateHideoutMinY;
        }

        // Instantiate the Pirate Hideout at the specified position.
        GameObject pirateHideout = Instantiate(pirateHideoutPrefab, new Vector3(x, 0, y), Quaternion.identity);
        pirateHideout.transform.SetParent(transform);

        // Position the Pirate Hideout correctly in the grid.
        PositionPirateHideout(pirateHideout, x, y);

        // Track the instantiated pirate hideout GameObject.
        activePirateHideouts.Add(pirateHideout);

        // Add the position to the pirate hideout positions set.
        PlacePirateHideout(x, y);
    }

    public void PositionPirateHideout(GameObject pirateHideout, int x, int y, bool force = false)
    {
        // Calculate the center position of the grid cell
        Vector3 tileCenter = GetTileCenter(x, y);

        // Set the position of the Pirate Hideout prefab
        pirateHideout.transform.position = tileCenter;
    }
    private bool IsPositionOccupied(int x, int y)
    {
        // Check if the position is within bounds and has a ship.
        if (x < 0 || x >= TILE_COUNT_X || y < 0 || y >= TILE_COUNT_Y)
            return true; // Consider out-of-bounds as occupied.

        return shipboardPieces[x, y] != null;
    }

    private void SpawnRandomCalamity()
    {
        // Randomly decide to spawn either a reef (0) or a whirlpool (1)
        int calamityType = mt.Next(5); // Generates 0, 1, 2, 3, 4

        if (calamityType == 0)
        {
            SpawnRandomReef(); // Spawn a reef
        }
        else if (calamityType == 1)
        {
            SpawnRandomWaterspout();
        }
        else if (calamityType == 2)
        {
            SpawnRandomTreacherousCurrent();
        }
        else if (calamityType == 3)
        {
            SpawnRandomPirateHideout();
        }
        else
        {
            SpawnRandomWhirlpool(); // Spawn a whirlpool
        }
    }
    private void ClearCalamities()
    {
        // Deactivate or remove whirlpool-related objects and reset related data.
        foreach (GameObject whirlpool in activeWhirlpools)
        {
            Destroy(whirlpool); // Destroy or deactivate the whirlpool GameObject.
        }
        activeWhirlpools.Clear(); // Clear the whirlpool object list.
        whirlpoolTiles.Clear(); // Clear the whirlpool tile data.
        whirlpoolCenterPosition.Clear(); // Clear whirlpool center position data.

        // Deactivate or remove waterspout-related objects and reset related data.
        foreach (GameObject waterspout in activeWaterspouts)
        {
            Destroy(waterspout); // Destroy or deactivate the waterspout GameObject.
        }
        activeWaterspouts.Clear(); // Clear the waterspout object list.
        waterspoutTiles.Clear(); // Clear the waterspout tile data.
        waterspoutCenterPosition.Clear(); // Clear waterspout center position data.

        // Deactivate or remove reef-related objects and reset related data.
        foreach (GameObject reef in activeReefs)
        {
            Destroy(reef); // Destroy or deactivate the reef GameObject.
        }
        activeReefs.Clear(); // Clear the reef object list.
        reefPositions.Clear(); // Clear the reef tile positions data.

        // Deactivate or remove reef-related objects and reset related data.
        foreach (GameObject treacherousCurrent in activeTreacherousCurrents)
        {
            Destroy(treacherousCurrent); // Destroy or deactivate the reef GameObject.
        }
        activeTreacherousCurrents.Clear(); // Clear the reef object list.
        treacherousCurrentPositions.Clear(); // Clear the reef tile positions data.

        foreach (GameObject pirateHideout in activePirateHideouts)
        {
            Destroy(pirateHideout); // Destroy or deactivate the reef GameObject.
        }
        activePirateHideouts.Clear(); // Clear the reef object list.
        pirateHideoutPositions.Clear(); // Clear the reef tile positions data.
    }

    //Dice
    // Award skill points to the current player based on dice face number
    private void AwardSkillPoints(int diceFace)
    {
        // Ensure diceFace is within the valid range (1-6)
        if (diceFace < 1 || diceFace > 6)
        {
            Debug.LogError("Invalid dice face detected: " + diceFace);
            return;
        }

        int skillPoints = diceFace; // Dice face number equals the skill points

        // Award the skill points based on isPlayer1Turn
        if (isPlayer1Turn) // If it's player 2 turn
        {
            player2_SP += skillPoints;
            Debug.Log("Player 2 gains " + skillPoints + " skill points.");
        }
        else // Player 1 gains skill
        {
            player1_SP += skillPoints;
            Debug.Log("Player 1 gains " + skillPoints + " skill points.");
        }
    }


    //Getters and Setters

}