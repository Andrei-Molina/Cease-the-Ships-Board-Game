using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class VolcanoCountdownUI : MonoBehaviour
{
    public GameObject countdownUIPrefab; // Assign the VolcanoCountdownUI prefab
    public Transform uiParent; // Assign the canvas or relevant UI parent
    private Shipboard shipboard;

    private void Awake()
    {
        shipboard = FindObjectOfType<Shipboard>();
    }

    public void ShowVolcanoCountdown(int turnsRemaining)
    {
        // Instantiate the countdown UI prefab
        GameObject countdownUI = Instantiate(countdownUIPrefab, uiParent);

        // Update the text
        TextMeshProUGUI textComponent = countdownUI.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = $"{turnsRemaining} Turns Until Volcano Erupts";

        // Set the position of the RectTransform to the center of the screen
        RectTransform canvasRect = uiParent.GetComponent<RectTransform>();
        RectTransform countdownRect = countdownUI.GetComponent<RectTransform>();
        countdownRect.localPosition = Vector3.zero; // Center of the canvas

        // Optional: Ensure anchor is also centered for better alignment
        //countdownRect.anchorMin = new Vector2(0.5f, 0.5f);
        //countdownRect.anchorMax = new Vector2(0.5f, 0.5f);
        countdownRect.pivot = new Vector2(0.5f, 0.5f);

        // Destroy the countdown UI after 3 seconds
        Destroy(countdownUI, 3f);
    }


}
