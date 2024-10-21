using UnityEngine;

public class SettingsPanelManager : MonoBehaviour
{
    private static SettingsPanelManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            // This makes the SettingsPanel persist between scene changes
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If there's already an instance, destroy the duplicate
            Destroy(gameObject);
        }
    }
}
