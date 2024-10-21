using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpandableDeadShipController : MonoBehaviour
{
    public GameObject buttonContainer;
    public GameObject[] ships; // Backgrounds array (Red, Blue, Silver, Black)
    public Button activateButton; // Activate button (drag to Inspector)

    // Use existing buttons for Player 1 and Player 2 dead ship
    public Button player1DeadShipButton;
    public Button player2DeadShipButton;

    // Sprites for Player 1
    public Sprite redSprite1;
    public Sprite blueSprite1;
    public Sprite silverSprite1;
    public Sprite blackSprite1;

    // Sprites for Player 2
    public Sprite redSprite2;
    public Sprite blueSprite2;
    public Sprite silverSprite2;
    public Sprite blackSprite2;

    public TextMeshProUGUI destroyedShipsText;

    public Shipboard shipboard;

    public GameObject player1DeadShip;
    public GameObject player2DeadShip;

    public float targetHeight = 382.8596f; // Desired height for background expansion
    public float lerpDuration = 0.9f; // Lerp duration for background expansion

    private RectTransform[] rectTransforms;
    private Vector2 initialSize;
    public static bool isExpanded = false;

    private Color player1Color;
    private Color player2Color;
    private bool isPlayer1Turn;
    private bool isPlayer1Active = true; // Track which player's ship is active

    private void Start()
    {
        shipboard = FindObjectOfType<Shipboard>();
        if (ships != null && ships.Length > 0)
        {
            rectTransforms = new RectTransform[ships.Length];
            initialSize = new Vector2(1050.345f, 0); // Initial size (width is fixed)

            // Initialize backgrounds (ships)
            for (int i = 0; i < ships.Length; i++)
            {
                rectTransforms[i] = ships[i].GetComponent<RectTransform>();
                rectTransforms[i].sizeDelta = initialSize; // Set initial size
                ships[i].SetActive(false); // Start inactive
            }

            // Set up button listeners
            activateButton.onClick.AddListener(OnButtonClick);

            // Attach listeners to switch between Player 1 and Player 2 dead ships
            player1DeadShipButton.onClick.AddListener(SwitchToPlayer1DeadShip);
            player2DeadShipButton.onClick.AddListener(SwitchToPlayer2DeadShip);

            // Get player colors from GameManager
            player1Color = GameManager.instance.player1Color;
            player2Color = GameManager.instance.player2Color;

            // Initialize dead ships and destroyed ships text
            player1DeadShip.SetActive(false);
            player2DeadShip.SetActive(false);
            destroyedShipsText.gameObject.SetActive(false);

            // Setup player buttons based on color and sprite
            SetUpPlayerButtons();
        }
    }

    private void SetUpPlayerButtons()
    {
        // Set the sprite and functionality for Player 1's dead ship button
        if (player1Color == Color.red)
        {
            player1DeadShipButton.image.sprite = redSprite1;
            player1DeadShipButton.onClick.RemoveAllListeners();
            player1DeadShipButton.onClick.AddListener(() => ActivateBackgroundAndShip(player1Color, player1DeadShip));
        }
        else if (player1Color == Color.blue)
        {
            player1DeadShipButton.image.sprite = blueSprite1;
            player1DeadShipButton.onClick.RemoveAllListeners();
            player1DeadShipButton.onClick.AddListener(() => ActivateBackgroundAndShip(player1Color, player1DeadShip));
        }
        else if (player1Color == Color.gray)
        {
            player1DeadShipButton.image.sprite = silverSprite1;
            player1DeadShipButton.onClick.RemoveAllListeners();
            player1DeadShipButton.onClick.AddListener(() => ActivateBackgroundAndShip(player1Color, player1DeadShip));
        }
        else if (player1Color == Color.black)
        {
            player1DeadShipButton.image.sprite = blackSprite1;
            player1DeadShipButton.onClick.RemoveAllListeners();
            player1DeadShipButton.onClick.AddListener(() => ActivateBackgroundAndShip(player1Color, player1DeadShip));
        }

        // Set the sprite and functionality for Player 2's dead ship button
        if (player2Color == Color.red)
        {
            player2DeadShipButton.image.sprite = redSprite2;
            player2DeadShipButton.onClick.RemoveAllListeners();
            player2DeadShipButton.onClick.AddListener(() => ActivateBackgroundAndShip(player2Color, player2DeadShip));
        }
        else if (player2Color == Color.blue)
        {
            player2DeadShipButton.image.sprite = blueSprite2;
            player2DeadShipButton.onClick.RemoveAllListeners();
            player2DeadShipButton.onClick.AddListener(() => ActivateBackgroundAndShip(player2Color, player2DeadShip));
        }
        else if (player2Color == Color.gray)
        {
            player2DeadShipButton.image.sprite = silverSprite2;
            player2DeadShipButton.onClick.RemoveAllListeners();
            player2DeadShipButton.onClick.AddListener(() => ActivateBackgroundAndShip(player2Color, player2DeadShip));
        }
        else if (player2Color == Color.black)
        {
            player2DeadShipButton.image.sprite = blackSprite2;
            player2DeadShipButton.onClick.RemoveAllListeners();
            player2DeadShipButton.onClick.AddListener(() => ActivateBackgroundAndShip(player2Color, player2DeadShip));
        }
    }

    private void OnButtonClick()
    {
        isExpanded = !isExpanded; // Toggle expansion state

        if (isExpanded)
        {
            // Check whose turn it is when the button is clicked
            isPlayer1Turn = shipboard.GetIsPlayer1Turn();

            // Set the correct background and dead ship based on whose turn it is
            if (isPlayer1Turn)
            {
                isPlayer1Active = true; // Player 1 is active
                ActivateBackgroundAndShip(player1Color, player1DeadShip); // Activate Player 1's dead ship and background
            }
            else
            {
                isPlayer1Active = false; // Player 2 is active
                ActivateBackgroundAndShip(player2Color, player2DeadShip); // Activate Player 2's dead ship and background
            }

            // Activate the appropriate switch buttons
            SetUpPlayerButtons();
            buttonContainer.gameObject.SetActive(true);
        }
        else
        {
            // Collapse the backgrounds
            foreach (var background in ships)
            {
                background.SetActive(false);
            }

            player1DeadShip.SetActive(false);
            player2DeadShip.SetActive(false);
            destroyedShipsText.gameObject.SetActive(false);
            buttonContainer.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isExpanded)
        {
            // Lerp the background expansion
            float lerpSpeed = 200f;
            foreach (var rectTransform in rectTransforms)
            {
                float newHeight = Mathf.MoveTowards(rectTransform.sizeDelta.y, targetHeight, lerpSpeed);
                rectTransform.sizeDelta = new Vector2(initialSize.x, newHeight);

                if (Mathf.Abs(newHeight - targetHeight) < 0.01f)
                {
                    rectTransform.sizeDelta = new Vector2(initialSize.x, targetHeight);
                }
            }

            destroyedShipsText.gameObject.SetActive(true);

            // Activate the appropriate dead ship based on the active player
            if (isPlayer1Active)
            {
                ActivateBackgroundAndShip(player1Color, player1DeadShip);
            }
            else
            {
                ActivateBackgroundAndShip(player2Color, player2DeadShip);
            }
        }
        else
        {
            // Collapse the backgrounds
            foreach (var rectTransform in rectTransforms)
            {
                float newHeight = Mathf.MoveTowards(rectTransform.sizeDelta.y, 0, 200f);
                rectTransform.sizeDelta = new Vector2(initialSize.x, newHeight);

                if (Mathf.Abs(newHeight - 0) < 0.01f)
                {
                    rectTransform.sizeDelta = new Vector2(initialSize.x, 0);
                    rectTransform.gameObject.SetActive(false);
                }
            }

            destroyedShipsText.gameObject.SetActive(false);
        }
    }

    public void SwitchToPlayer1DeadShip()
    {
        isPlayer1Active = true; // Set Player 1 as active
        ActivateBackgroundAndShip(player1Color, player1DeadShip);
    }

    public void SwitchToPlayer2DeadShip()
    {
        isPlayer1Active = false; // Set Player 2 as active
        ActivateBackgroundAndShip(player2Color, player2DeadShip);
    }

    private void ActivateBackgroundAndShip(Color playerColor, GameObject playerDeadShip)
    {
        // Deactivate all backgrounds
        foreach (var background in ships)
        {
            background.SetActive(false);
        }

        // Deactivate both player dead ships
        player1DeadShip.SetActive(false);
        player2DeadShip.SetActive(false);

        // Activate the correct background based on the player's color
        if (playerColor == Color.red)
        {
            ships[0].SetActive(true); // Red background
        }
        else if (playerColor == Color.blue)
        {
            ships[1].SetActive(true); // Blue background
        }
        else if (playerColor == Color.gray)
        {
            ships[2].SetActive(true); // Silver background
        }
        else if (playerColor == Color.black)
        {
            ships[3].SetActive(true); // Black background
        }

        // Activate the player's dead ship
        playerDeadShip.SetActive(true);
    }
}
