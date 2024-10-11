using UnityEngine;

public class WaterSpoutBehavior : MonoBehaviour
{
    private Shipboard shipboard;

    // Called when the script is first run
    void Awake()
    {
        // Initialize shipboard in Awake method
        shipboard = FindObjectOfType<Shipboard>();
        if (shipboard == null)
        {
            Debug.LogError("Shipboard not found in the scene.");
        }
    }
}
