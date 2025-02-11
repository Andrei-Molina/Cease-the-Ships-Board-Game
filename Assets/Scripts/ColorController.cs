using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ColorController : MonoBehaviour
{
    public Button[] player1ColorButtons;
    public Button[] player2ColorButtons;
    public Button[] aiColorButtons;

    public Color[] availableColors;

    // Player 1 Ships (FBX models, no CanvasGroup)
    public GameObject Ship1Black;
    public GameObject Ship1Silver;
    public GameObject Ship1Red;
    public GameObject Ship1Blue;

    // Player 2 Ships (FBX models, no CanvasGroup)
    public GameObject Ship2Black;
    public GameObject Ship2Silver;
    public GameObject Ship2Red;
    public GameObject Ship2Blue;

    public GameObject ShipAIBlack;
    public GameObject ShipAISilver;
    public GameObject ShipAIRed;
    public GameObject ShipAIBlue;

    // CanvasGroup for fading UI (e.g., RawImage containing the CanvasGroup)
    public CanvasGroup player1CanvasGroup;
    public CanvasGroup player2CanvasGroup;
    public CanvasGroup player3CanvasGroup;

    private GameObject currentPlayer1Ship;
    private GameObject currentPlayer2Ship;
    private GameObject currentAIShip;

    private Color? currentPlayer1Color;
    private Color? currentPlayer2Color;
    private Color? currentAIColor;

    public float transitionDuration = 1.0f;

    private static ColorController instance;

    private void Awake()
    {

    }

    void Start()
    {
        availableColors = new Color[] { Color.black, Color.gray, Color.red, Color.blue };

        for (int i = 0; i < player1ColorButtons.Length; i++)
        {
            int index = i;
            player1ColorButtons[i].onClick.AddListener(() => SelectPlayer1Color(availableColors[index], index));
        }

        for (int i = 0; i < player2ColorButtons.Length; i++)
        {
            int index = i;
            player2ColorButtons[i].onClick.AddListener(() => SelectPlayer2Color(availableColors[index], index));
        }


        for (int i = 0; i < aiColorButtons.Length; i++)
        {
            int index = i;
            aiColorButtons[i].onClick.AddListener(() => SelectAIColor(availableColors[index], index));
        }
    }

    public void SelectPlayer1Color(Color selectedColor, int index)
    {
        if (currentPlayer1Color == selectedColor)
        {
            Debug.Log("Player 1 clicked the same color again: " + selectedColor);
            return;
        }

        GameManager.instance.player1Color = selectedColor;
        currentPlayer1Color = selectedColor;

        // Get the new ship based on the index
        GameObject newShip = GetPlayer1ShipByIndex(index);

        // Start the transition on the RawImage CanvasGroup and swap the ship model
        StartCoroutine(TransitionShip(player1CanvasGroup, currentPlayer1Ship, newShip));

        currentPlayer1Ship = newShip;

        UpdateButtonInteractivity();
    }

    public void SelectPlayer2Color(Color selectedColor, int index)
    {
        if (currentPlayer2Color == selectedColor)
        {
            Debug.Log("Player 2 clicked the same color again: " + selectedColor);
            return;
        }

        GameManager.instance.player2Color = selectedColor;
        currentPlayer2Color = selectedColor;

        // Get the new ship based on the index
        GameObject newShip = GetPlayer2ShipByIndex(index);

        // Start the transition on the RawImage CanvasGroup and swap the ship model
        StartCoroutine(TransitionShip(player2CanvasGroup, currentPlayer2Ship, newShip));

        currentPlayer2Ship = newShip;

        UpdateButtonInteractivity();
    }

    public void SelectAIColor(Color selectedColor, int index)
    {
        if (currentAIColor == selectedColor)
        {
            Debug.Log("AI clicked the same color again: " + selectedColor);
            return;
        }

        GameManager.instance.aiColor = selectedColor;
        currentAIColor = selectedColor;

        GameObject newShip = GetAIShipByIndex(index);
        StartCoroutine(TransitionShip(player3CanvasGroup, currentAIShip, newShip));

        currentAIShip = newShip;

        UpdateButtonInteractivity();
    }

    private void UpdateButtonInteractivity()
    {
        for (int i = 0; i < availableColors.Length; i++)
        {
            bool isDisabled =
                (currentPlayer1Color.HasValue && availableColors[i] == currentPlayer1Color.Value) ||
                (currentPlayer2Color.HasValue && availableColors[i] == currentPlayer2Color.Value) ||
                (currentAIColor.HasValue && availableColors[i] == currentAIColor.Value);

            player1ColorButtons[i].interactable = !isDisabled;
            player2ColorButtons[i].interactable = !isDisabled;
            aiColorButtons[i].interactable = !isDisabled;
        }
    }

    // Coroutine for fading between two ships using the CanvasGroup for UI fade
    private IEnumerator TransitionShip(CanvasGroup canvasGroup, GameObject toDeactivate, GameObject toActivate)
    {
        // Fade out the CanvasGroup (the UI element, not the ships)
        if (canvasGroup != null)
        {
            float fadeOutTime = 0;
            while (fadeOutTime < transitionDuration)
            {
                fadeOutTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1, 0, fadeOutTime / transitionDuration);
                yield return null;
            }

            canvasGroup.alpha = 0;
        }

        // Deactivate the current ship model (FBX)
        if (toDeactivate != null)
        {
            toDeactivate.SetActive(false);
        }

        // Activate the new ship model (FBX)
        if (toActivate != null)
        {
            toActivate.SetActive(true);
        }

        // Fade in the CanvasGroup (the UI element)
        if (canvasGroup != null)
        {
            float fadeInTime = 0;
            while (fadeInTime < transitionDuration)
            {
                fadeInTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, fadeInTime / transitionDuration);
                yield return null;
            }

            canvasGroup.alpha = 1;
        }
    }

    private GameObject GetPlayer1ShipByIndex(int index)
    {
        switch (index)
        {
            case 0: return Ship1Black;
            case 1: return Ship1Silver;
            case 2: return Ship1Red;
            case 3: return Ship1Blue;
            default: return null;
        }
    }

    private GameObject GetPlayer2ShipByIndex(int index)
    {
        switch (index)
        {
            case 0: return Ship2Black;
            case 1: return Ship2Silver;
            case 2: return Ship2Red;
            case 3: return Ship2Blue;
            default: return null;
        }
    }

    private GameObject GetAIShipByIndex(int index)
    {
        switch (index)
        {
            case 0: return ShipAIBlack;
            case 1: return ShipAISilver;
            case 2: return ShipAIRed;
            case 3: return ShipAIBlue;
            default: return null;
        }
    }

    private void UpdatePlayer2ButtonInteractivity(Color selectedColor, bool isDisabled)
    {
        // Enable all buttons for Player 1, Player 2, and AI first
        foreach (var button in player1ColorButtons)
        {
            button.interactable = true;
        }

        foreach (var button in player2ColorButtons)
        {
            button.interactable = true;
        }

        foreach (var button in aiColorButtons)
        {
            button.interactable = true;
        }

        //Disable button corresponding to selected color of Player 1
        for (int i = 0; i < availableColors.Length; i++)
        {
            if (availableColors[i] == selectedColor)
            {
                player1ColorButtons[i].interactable = !isDisabled;
                player2ColorButtons[i].interactable = !isDisabled;
                aiColorButtons[i].interactable = !isDisabled;
            }
        }
    }
    // Validate that both players have selected a color before starting the game
    public bool AreColorsSelected()
    {
        if (currentPlayer1Color == null || currentPlayer2Color == null)
        {
            Debug.LogError("Both players must select a color before starting the game!");
            return false;
        }
        return true;
    }

    // Check if Player 1 has selected a color
    public bool IsPlayer1ColorSelected()
    {
        if (currentPlayer1Color == null)
        {
            Debug.LogError("Player 1 must select a color before proceeding!");
            return false;
        }
        return true;
    }

    // Check if Player 2 has selected a color
    public bool IsPlayer2ColorSelected()
    {
        if (currentPlayer2Color == null)
        {
            Debug.LogError("Player 2 must select a color before starting the game!");
            return false;
        }
        return true;
    }
    public bool IsAIColorSelected()
    {
        if (currentAIColor == null)
        {
            Debug.LogError("AI must select a color before starting the game!");
            return false;
        }
        return true;
    }
    public void ResetColorButtonsInteractability()
    {
        currentPlayer2Color = null;
        currentAIColor = null;
        GameManager.instance.aiColor = Color.clear;
        GameManager.instance.player2Color = Color.clear;

        // Deactivate the current Player 2 ship model
        if (currentPlayer2Ship != null)
        {
            currentPlayer2Ship.SetActive(false);
        }

        // Deactivate the current Player 2 ship model
        if (currentAIShip != null)
        {
            currentAIShip.SetActive(false);
        }

        // Enable all Player 2 and AI color buttons
        foreach (var button in player2ColorButtons)
        {
            button.interactable = true;
        }

        foreach (var button in aiColorButtons)
        {
            button.interactable = true;
        }

        // Check Player 1's current color and disable corresponding buttons
        if (currentPlayer1Color != null)
        {
            UpdatePlayer2ButtonInteractivity(currentPlayer1Color.Value, true);
        }
    }

    public void ResetAllColorButtonsInteractability()
    {
        currentPlayer1Color = null;
        currentPlayer2Color = null;
        currentAIColor = null;
        GameManager.instance.player1Color = Color.clear;
        GameManager.instance.aiColor = Color.clear;
        GameManager.instance.player2Color = Color.clear;

        // Deactivate the current Player 1 ship model
        if (currentPlayer1Ship != null)
        {
            currentPlayer1Ship.SetActive(false);
        }

        // Deactivate the current Player 2 ship model
        if (currentPlayer2Ship != null)
        {
            currentPlayer2Ship.SetActive(false);
        }

        // Deactivate the current AI ship model
        if (currentAIShip != null)
        {
            currentAIShip.SetActive(false);
        }

        // Enable all Player 1 color buttons
        foreach (var button in player1ColorButtons)
        {
            button.interactable = true;
        }

        // Enable all Player 2 color buttons
        foreach (var button in player2ColorButtons)
        {
            button.interactable = true;
        }
        // Enable all AI color buttons
        foreach (var button in aiColorButtons)
        {
            button.interactable = true;
        }
    }
}
