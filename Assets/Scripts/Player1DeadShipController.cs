using UnityEngine;
using UnityEngine.UI;

public class Player1DeadShipController : MonoBehaviour
{
    public GameObject player1DeadShip; // Assign in Inspector
    public Button activateButton; // Assign in Inspector
    public float targetHeight = 396.8436f; // Desired height
    public float lerpDuration = 0.9f; // Duration of the lerp
    private RectTransform rectTransform;
    private Vector2 initialSize;
    private bool isExpanded = false; // Track whether the ship is expanded

    void Start()
    {
        if (player1DeadShip != null)
        {
            rectTransform = player1DeadShip.GetComponent<RectTransform>();
            initialSize = new Vector2(542.6229f, 0); // Starting size (width is constant)
            rectTransform.sizeDelta = initialSize; // Set initial size
            player1DeadShip.SetActive(false); // Start inactive

            // Set up the button listener
            activateButton.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        if (!player1DeadShip.activeSelf)
        {
            player1DeadShip.SetActive(true);
            isExpanded = true; // Mark as expanded
        }
        else
        {
            isExpanded = false; // Mark as collapsing
        }
    }

    void Update()
    {
        if (player1DeadShip.activeSelf)
        {
            // Set a fast lerp speed
            float lerpSpeed = 200f; // Adjust this value for a faster effect

            float targetHeightValue = isExpanded ? targetHeight : 0; // Determine target height based on state
            float newHeight = Mathf.MoveTowards(rectTransform.sizeDelta.y, targetHeightValue, lerpSpeed);
            rectTransform.sizeDelta = new Vector2(initialSize.x, newHeight); // Keep width constant

            // Ensure it reaches the target height
            if (Mathf.Abs(newHeight - targetHeightValue) < 0.01f && !isExpanded)
            {
                player1DeadShip.SetActive(false); // Deactivate the ship when collapsing is complete
            }
        }
    }
}
