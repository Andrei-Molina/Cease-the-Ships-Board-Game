using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogsBackgroundManager : MonoBehaviour
{
    public GameObject player1Background;  // Reference to Player 1 background object
    public GameObject player2Background;  // Reference to Player 2 background object

    public Sprite redBackgroundSprite;    // Sprite for red background
    public Sprite blueBackgroundSprite;   // Sprite for blue background
    public Sprite blackBackgroundSprite;  // Sprite for green background
    public Sprite silverBackgroundSprite; // Sprite for yellow background

    [SerializeField] private GameObject logsCanvas;
    [SerializeField] private GameObject logsBackground;

    private Shipboard shipboard;

    private void Start()
    {
        shipboard = FindObjectOfType<Shipboard>();
        // Set the background sprites when the game starts
        UpdateBackgrounds();
    }

    public void UpdateBackgrounds()
    {
        // Update Player 1 Background
        UpdatePlayerBackground(GameManager.instance.player1Color, player1Background);

        // Update Player 2 Background
        UpdatePlayerBackground(GameManager.instance.player2Color, player2Background);
    }

    private void UpdatePlayerBackground(Color playerColor, GameObject backgroundObject)
    {
        Image backgroundImage = backgroundObject.GetComponent<Image>();

        if (playerColor == Color.red)
        {
            backgroundImage.sprite = redBackgroundSprite;
        }
        else if (playerColor == Color.blue)
        {
            backgroundImage.sprite = blueBackgroundSprite;
        }
        else if (playerColor == Color.black)
        {
            backgroundImage.sprite = blackBackgroundSprite;
        }
        else if (playerColor == Color.gray)
        {
            backgroundImage.sprite = silverBackgroundSprite;
        }
        else
        {
            // Set a default sprite or keep the existing one if no match
            Debug.LogWarning("Color not recognized, keeping default background.");
        }
    }
    public void ShowLogs()
    {
        if (!logsCanvas.activeSelf)
        {
            logsBackground.gameObject.SetActive(true);
            logsCanvas.gameObject.SetActive(true);
            player1Background.gameObject.SetActive(true);
            player2Background.gameObject.SetActive(true);
        }

        else if (logsCanvas.activeSelf)
        {
            logsBackground.gameObject.SetActive(false);
            logsCanvas.gameObject.SetActive(false);
            player1Background.gameObject.SetActive(false);
            player2Background.gameObject.SetActive(false);
        }

        if (shipboard.GetIsPlayer1Turn())
        {
            player1Background.gameObject.SetActive(true);
            player2Background.gameObject.SetActive(false);
        }
        else
        {
            player1Background.gameObject.SetActive(false);
            player2Background.gameObject.SetActive(true);
        }
    }
}
