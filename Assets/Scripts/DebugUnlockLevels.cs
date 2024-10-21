using UnityEngine;

public class DebugUnlockLevels : MonoBehaviour
{
    private const string LEVEL_UNLOCK_KEY = "UnlockedLevel";
    private const string LEVEL_UNLOCK_KEY_MEDIUM = "UnlockedLevelMedium";
    private const string LEVEL_UNLOCK_KEY_HARD = "UnlockedLevelHard";

    // Unlock specific levels in Easy difficulty using different methods
    [ContextMenu("Unlock Easy Level 1")]
    public void UnlockEasyLevel1()
    {
        UnlockEasyLevel(1);
    }
    [ContextMenu("Unlock Easy Level 2")]
    public void UnlockEasyLevel2()
    {
        UnlockEasyLevel(2);
    }
    [ContextMenu("Unlock Easy Level 3")]
    public void UnlockEasyLevel3()
    {
        UnlockEasyLevel(3);
    }
    [ContextMenu("Unlock Easy Level 4")]
    public void UnlockEasyLevel4()
    {
        UnlockEasyLevel(4);
    }
    [ContextMenu("Unlock Easy Level 5")]
    public void UnlockEasyLevel5()
    {
        UnlockEasyLevel(5);
    }
    [ContextMenu("Unlock Easy Level 6")]
    public void UnlockEasyLevel6()
    {
        UnlockEasyLevel(6);
    }
    [ContextMenu("Unlock Easy Level 7")]
    public void UnlockEasyLevel7()
    {
        UnlockEasyLevel(7);
    }
    [ContextMenu("Unlock Easy Level 8")]
    public void UnlockEasyLevel8()
    {
        UnlockEasyLevel(8);
    }
    [ContextMenu("Unlock Easy Level 9")]
    public void UnlockEasyLevel9()
    {
        UnlockEasyLevel(9);
    }
    [ContextMenu("Unlock Easy Level 10")]
    public void UnlockEasyLevel10()
    {
        UnlockEasyLevel(10);
    }
    // Unlock specific levels in Medium difficulty using different methods
    [ContextMenu("Unlock Medium Level 1")]
    public void UnlockMediumLevel1()
    {
        UnlockMediumLevel(1);
    }
    [ContextMenu("Unlock Medium Level 2")]
    public void UnlockMediumLevel2()
    {
        UnlockMediumLevel(2);
    }
    [ContextMenu("Unlock Medium Level 3")]
    public void UnlockMediumLevel3()
    {
        UnlockMediumLevel(3);
    }
    [ContextMenu("Unlock Medium Level 4")]
    public void UnlockMediumLevel4()
    {
        UnlockMediumLevel(4);
    }
    [ContextMenu("Unlock Medium Level 5")]
    public void UnlockMediumLevel5()
    {
        UnlockMediumLevel(5);
    }
    [ContextMenu("Unlock Medium Level 6")]
    public void UnlockMediumLevel6()
    {
        UnlockMediumLevel(6);
    }
    [ContextMenu("Unlock Medium Level 7")]
    public void UnlockMediumLevel7()
    {
        UnlockMediumLevel(7);
    }
    [ContextMenu("Unlock Medium Level 8")]
    public void UnlockMediumLevel8()
    {
        UnlockMediumLevel(8);
    }
    [ContextMenu("Unlock Medium Level 9")]
    public void UnlockMediumLevel9()
    {
        UnlockMediumLevel(9);
    }
    [ContextMenu("Unlock Medium Level 10")]
    public void UnlockMediumLevel10()
    {
        UnlockMediumLevel(10);
    }
    // Unlock specific levels in Hard difficulty using different methods
    [ContextMenu("Unlock Hard Level 1")]
    public void UnlockHardLevel1()
    {
        UnlockHardLevel(1);
    }
    [ContextMenu("Unlock Hard Level 2")]
    public void UnlockHardLevel2()
    {
        UnlockHardLevel(2);
    }
    [ContextMenu("Unlock Hard Level 3")]
    public void UnlockHardLevel3()
    {
        UnlockHardLevel(3);
    }
    [ContextMenu("Unlock Hard Level 4")]
    public void UnlockHardLevel4()
    {
        UnlockHardLevel(4);
    }
    [ContextMenu("Unlock Hard Level 5")]
    public void UnlockHardLevel5()
    {
        UnlockHardLevel(5);
    }
    [ContextMenu("Unlock Hard Level 6")]
    public void UnlockHardLevel6()
    {
        UnlockHardLevel(6);
    }
    [ContextMenu("Unlock Hard Level 7")]
    public void UnlockHardLevel7()
    {
        UnlockHardLevel(7);
    }
    [ContextMenu("Unlock Hard Level 8")]
    public void UnlockHardLevel8()
    {
        UnlockHardLevel(8);
    }
    [ContextMenu("Unlock HardLevel 9")]
    public void UnlockHardLevel9()
    {
        UnlockHardLevel(9);
    }
    [ContextMenu("Unlock Hard Level 10")]
    public void UnlockHardLevel10()
    {
        UnlockHardLevel(10);
    }
    // Method to unlock a specific level
    public void UnlockEasyLevel(int level)
    {
        PlayerPrefs.SetInt(LEVEL_UNLOCK_KEY, level);
        PlayerPrefs.Save();
        Debug.Log($"Level {level} unlocked.");
    }
    public void UnlockMediumLevel(int level)
    {
        PlayerPrefs.SetInt(LEVEL_UNLOCK_KEY_MEDIUM, level);
        PlayerPrefs.Save();
        Debug.Log($"Level {level} unlocked.");
    }
    public void UnlockHardLevel(int level)
    {
        PlayerPrefs.SetInt(LEVEL_UNLOCK_KEY_HARD, level);
        PlayerPrefs.Save();
        Debug.Log($"Level {level} unlocked.");
    }

    // Method to reset all progress (lock all levels)
    [ContextMenu("Reset Unlocked Levels")]
    public void ResetUnlockedLevels()
    {
        PlayerPrefs.SetInt(LEVEL_UNLOCK_KEY, 1); // Reset to only unlock level 1
        PlayerPrefs.Save();
        Debug.Log("Progress reset: Only Level 1 is unlocked.");
    }

    // Test method that logs the currently unlocked level
    [ContextMenu("Print Unlocked Level")]
    public void PrintUnlockedLevel()
    {
        int unlockedLevel = PlayerPrefs.GetInt(LEVEL_UNLOCK_KEY, 1);
        Debug.Log($"Currently unlocked level: {unlockedLevel}");
    }

    // You can use this method to clear PlayerPrefs if necessary
    [ContextMenu("Clear All PlayerPrefs")]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared.");
    }
}
