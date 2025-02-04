using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCalamities : MonoBehaviour
{
    [SerializeField] private GameObject waterspoutCenter;
    [SerializeField] private GameObject whirlpoolCenter;
    [SerializeField] private GameObject shipwreckCenter;
    [SerializeField] private Button toggleButton;
    [SerializeField] private Sprite[] toggleIcons;

    private bool isVisible = true;

    private void Start()
    {
        // Set the initial button icon
        UpdateButtonIcon();

        // Add listener once
        toggleButton.onClick.AddListener(Toggle);
    }

    private void Toggle()
    {
        if (toggleButton.GetComponent<Image>().color.a == 1)
        {
            isVisible = !isVisible;

            // Toggle the visibility of the waterspoutCenter and whirlpoolCenter
            if (waterspoutCenter != null)
            {
                waterspoutCenter.SetActive(isVisible);
            }

            if (whirlpoolCenter != null)
            {
                whirlpoolCenter.SetActive(isVisible);
            }

            // Update the button icon based on visibility
            UpdateButtonIcon();
        }
        else
        {
            Debug.Log("Toggle Button is not yet active");
        }
    }

    public void SetWaterspoutCenter (GameObject waterspout)
    {
        waterspoutCenter = waterspout;
    }

    public void SetWhirlpoolCenter (GameObject whirlpool)
    {
        whirlpoolCenter = whirlpool;
    }

    public void SetShipwreckCenter (GameObject shipwreck)
    {
        shipwreckCenter = shipwreck;
    }

    private void UpdateButtonIcon()
    {
        // Change the button's icon based on the current visibility state
        if (toggleIcons.Length > 0)
        {
            toggleButton.GetComponent<Image>().sprite = isVisible ? toggleIcons[0] : toggleIcons[1];
        }
    }

}
