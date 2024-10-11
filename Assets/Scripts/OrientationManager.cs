using UnityEngine;
using UnityEngine.UI; // Import UI namespace for CanvasScaler

public class OrientationManager : MonoBehaviour
{
    // Boolean to keep track of current orientation
    private bool isLandscape = true;

    // Reference to the main camera
    public Camera mainCamera;

    // Reference to the CanvasScaler for adjusting resolution
    public CanvasScaler canvasScaler;

    // Public property for isLandscape
    public bool IsLandscape
    {
        get { return isLandscape; }
        set { isLandscape = value; }
    }

    // Method to toggle orientation
    public void ToggleOrientation()
    {
        if (isLandscape)
        {
            // Switch to portrait mode
            Screen.orientation = ScreenOrientation.Portrait;

            // Update camera settings for portrait
            mainCamera.fieldOfView = 96f;
            mainCamera.transform.position = new Vector3(-0.140000001f, 6.48999977f, -5.0999999f);

            // Update canvas resolution for portrait
            canvasScaler.referenceResolution = new Vector2(1080f, 1920f); // Portrait resolution
        }
        else
        {
            // Switch to landscape mode
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            // Update camera settings for landscape
            mainCamera.fieldOfView = 86.7f;
            mainCamera.transform.position = new Vector3(-0.140000001f, 4.11000013f, -3.74000001f);

            // Update canvas resolution for landscape
            canvasScaler.referenceResolution = new Vector2(1920f, 1080f); // Landscape resolution
        }

        // Toggle the boolean
        isLandscape = !isLandscape;
    }

    // Ensure the main camera and canvas scaler references are set
    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (canvasScaler == null)
        {
            // Make sure a CanvasScaler is attached to the Canvas
            canvasScaler = FindObjectOfType<CanvasScaler>();
        }
    }
}
