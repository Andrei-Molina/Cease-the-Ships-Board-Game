using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public Button[] timerButtons; // Buttons for selecting timer
    public float[] timerDurations; // Array of durations corresponding to the buttons

    private float? selectedTimer = null; // Store the selected timer duration

    void Start()
    {
        // Ensure that the number of buttons and timer durations match
        if (timerButtons.Length != timerDurations.Length)
        {
            Debug.LogError("Number of buttons and timer durations do not match!");
            return;
        }

        // Assign listeners to each button to handle timer selection
        for (int i = 0; i < timerButtons.Length; i++)
        {
            int index = i;
            timerButtons[i].onClick.AddListener(() => SelectTimer(timerDurations[index], index));
        }
    }

    // Method to select a timer, disable the button, and store the value in GameManager
    public void SelectTimer(float timerDuration, int index)
    {
        if (selectedTimer != null && selectedTimer == timerDuration)
        {
            Debug.Log("Same timer clicked again: " + timerDuration);
            return;
        }

        // Reactivate all buttons first
        foreach (Button button in timerButtons)
        {
            button.interactable = true; // Make all buttons interactable
        }

        // Disable the clicked button
        timerButtons[index].interactable = false;

        // Store the selected timer in the GameManager
        GameManager.instance.selectedTimer = timerDuration;

        selectedTimer = timerDuration;
        Debug.Log("Timer set to: " + timerDuration + " minutes.");
    }
}
