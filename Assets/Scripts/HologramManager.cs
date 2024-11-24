using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // For scene loading
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class HologramManager : MonoBehaviour
{
    [System.Serializable]
    public struct HologramData
    {
        public GameObject hologramObject;
        public GameObject hologramFBX;
        public GameObject hologramRawImage;
        public Camera hologramCamera;
    }

    // List of all hologram data (submarine, aircraft carrier, and others)
    public List<HologramData> holograms;

    // List of inactive and active buttons for each hologram
    [SerializeField] private List<Button> buttonsInactive;
    [SerializeField] private List<Button> buttonsActive;

    [SerializeField] private string[] shipDescriptionTitle;
    [SerializeField] private string[] shipDescriptionBody;
    [SerializeField] private GameObject DescriptionBox;
    [SerializeField] private TextMeshProUGUI shipTitle;
    [SerializeField] private TextMeshProUGUI shipBody; 

    // State tracking for each hologram
    private List<bool> isHologramActive;

    public float rotationSpeed = 100f;
    private GameObject activeHologram;
    public float transitionDuration = 1.0f;
    private Vector2 previousTouchPosition;

    void Start()
    {
        // Initialize hologram active states
        isHologramActive = new List<bool>(new bool[holograms.Count]);
    }

    void Update()
    {
        HandleTouchInput();
    }

    // General method to handle hologram button click by index
    public void OnHologramButtonClicked(int index)
    {
        ClearUI();

        // Activate corresponding buttons and hologram
        buttonsActive[index].gameObject.SetActive(true);
        buttonsInactive[index].gameObject.SetActive(true);
        DescriptionBox.gameObject.SetActive(true);
        shipTitle.text = shipDescriptionTitle[index];
        shipBody.text = shipDescriptionBody[index];

        HologramData hologramData = holograms[index];
        hologramData.hologramObject.SetActive(true);
        hologramData.hologramCamera.gameObject.SetActive(true);
        hologramData.hologramFBX.SetActive(true);
        hologramData.hologramRawImage.SetActive(true);

        // Reactivate all inactive buttons except the active one (at index)
        for (int i = 0; i < buttonsInactive.Count; i++)
        {
            if (i != index) // Skip the active one
            {
                buttonsInactive[i].gameObject.SetActive(true);
            }
        }

        // If hologram is already active, no need to transition
        if (isHologramActive[index])
        {
            return;
        }

        // Trigger the transition effect between holograms
        StartCoroutine(TransitionHologram(hologramData.hologramRawImage, activeHologram != null ? activeHologram : null, false));

        activeHologram = hologramData.hologramObject;

        // Update the state for all holograms
        for (int i = 0; i < isHologramActive.Count; i++)
        {
            isHologramActive[i] = (i == index);
        }
    }

    // Handle touch input
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                RaycastToHologram(touch.position);
            }
        }
    }

    // Raycast to detect if user tapped on the active hologram
    private void RaycastToHologram(Vector2 touchPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject == activeHologram)
            {
                LoadBattlefieldScene();
            }
        }
    }

    // Load battlefield scene
    private void LoadBattlefieldScene()
    {
        SceneManager.LoadScene("Battlefield");
        Debug.Log("Battlefield Scene loaded");
    }

    // Transition between holograms
    private IEnumerator TransitionHologram(GameObject toActivate, GameObject toDeactivate, bool isSubmarine)
    {
        CanvasGroup activeGroup = toActivate?.GetComponent<CanvasGroup>();
        CanvasGroup inactiveGroup = toDeactivate?.GetComponent<CanvasGroup>();

        if (activeGroup != null)
        {
            activeGroup.alpha = 0;
            activeGroup.gameObject.SetActive(true); // Activate the new hologram
        }

        if (inactiveGroup != null && inactiveGroup.gameObject.activeSelf)
        {
            float fadeOutTime = 0;
            while (fadeOutTime < transitionDuration)
            {
                fadeOutTime += Time.deltaTime;
                inactiveGroup.alpha = Mathf.Lerp(1, 0, fadeOutTime / transitionDuration);
                yield return null;
            }

            inactiveGroup.gameObject.SetActive(false); // Deactivate the old hologram
        }

        float fadeInTime = 0;
        while (fadeInTime < transitionDuration)
        {
            fadeInTime += Time.deltaTime;
            activeGroup.alpha = Mathf.Lerp(0, 1, fadeInTime / transitionDuration);
            yield return null;
        }

        activeGroup.alpha = 1;
    }

    // Clear all holograms and buttons
    private void ClearUI()
    {
        foreach (var hologram in holograms)
        {
            hologram.hologramObject.SetActive(false);
            hologram.hologramCamera.gameObject.SetActive(false);
            hologram.hologramFBX.SetActive(false);
            hologram.hologramRawImage.SetActive(false);
        }

        foreach (var button in buttonsInactive)
        {
            button.gameObject.SetActive(false);
        }

        foreach (var button in buttonsActive)
        {
            button.gameObject.SetActive(false);
        }
    }
    public void ResetHologramStates()
    {
        for (int i = 0; i < isHologramActive.Count; i++)
        {
            isHologramActive[i] = false;
            holograms[i].hologramObject.SetActive(false); // Optionally deactivate hologram objects
        }

        activeHologram = null; // Reset the active hologram reference

        // Set all inactive buttons to true (visible) and all active buttons to false (hidden)
        for (int i = 0; i < buttonsInactive.Count; i++)
        {
            buttonsInactive[i].gameObject.SetActive(true);  // Show all inactive buttons
            buttonsActive[i].gameObject.SetActive(false);   // Hide all active buttons
        }

        DescriptionBox.gameObject.SetActive(false);
        shipBody.text = "";
        shipTitle.text = "";
    }

}
