using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShipboardSceneManager : MonoBehaviour
{
    // Background setup
    [SerializeField] private Image background; // Reference to the background image
    [SerializeField] private Sprite beachImage, treasureCaveImage, atollImage, sunkenCityImage, volcanicIslandImage, wastelandImage, lighthouseImage, murkySeasImage, cloudyAreaImage, stormyOceanImage, whirlIslandImage;
    [SerializeField] private GameObject beachWater, treasureCaveWater, atollWater, sunkenCityWater, volcanicIslandWater, wastelandWater, lighthouseWater, murkySeasWater, cloudyAreaWater, stormyOceanWater, whirlIslandWater;

    public Shipboard shipboard;

    // Calamity types
    private enum CalamityType { None, Reef, Whirlpool, TreacherousCurrent, PirateHideout, Waterspout };
    private Dictionary<int, List<CalamityType>> calamityByLevelEasy = new Dictionary<int, List<CalamityType>>
    {
        { 1, new List<CalamityType> { CalamityType.None } },
        { 2, new List<CalamityType> { CalamityType.Reef } },
        { 3, new List<CalamityType> { CalamityType.Reef, CalamityType.PirateHideout } },
        { 4, new List<CalamityType> { CalamityType.PirateHideout, CalamityType.TreacherousCurrent} },
        { 5, new List<CalamityType> { CalamityType.PirateHideout, CalamityType.TreacherousCurrent} },
        { 6, new List<CalamityType> { CalamityType.Reef} },
        { 7, new List<CalamityType> { CalamityType.TreacherousCurrent} },
        { 10, new List<CalamityType> { CalamityType.Whirlpool} },
    };
    private Dictionary<int, List<CalamityType>> calamityByLevelMedium = new Dictionary<int, List<CalamityType>>
    {
        { 1, new List<CalamityType> { CalamityType.PirateHideout } },
        { 2, new List<CalamityType> { CalamityType.TreacherousCurrent, CalamityType.PirateHideout } },
        { 6, new List<CalamityType> { CalamityType.TreacherousCurrent } },
        { 7, new List<CalamityType> { CalamityType.TreacherousCurrent, CalamityType.Waterspout } },
        { 8, new List<CalamityType> { CalamityType.Whirlpool, CalamityType.PirateHideout } },
        { 9, new List<CalamityType> { CalamityType.Whirlpool, CalamityType.PirateHideout } },
        { 10, new List<CalamityType> { CalamityType.TreacherousCurrent} }
    };
    private Dictionary<int, List<CalamityType>> calamityByLevelHard = new Dictionary<int, List<CalamityType>>
    {
        { 1, new List<CalamityType> { CalamityType.TreacherousCurrent, CalamityType.Whirlpool } },
        { 2, new List<CalamityType> { CalamityType.TreacherousCurrent, CalamityType.Whirlpool, CalamityType.Waterspout } },
        { 3, new List<CalamityType> { CalamityType.TreacherousCurrent, CalamityType.Waterspout, CalamityType.PirateHideout } },
        { 4, new List<CalamityType> { CalamityType.TreacherousCurrent, CalamityType.Waterspout, CalamityType.PirateHideout, CalamityType.Whirlpool } },
        { 5, new List<CalamityType> { CalamityType.TreacherousCurrent } },
        { 6, new List<CalamityType> { CalamityType.TreacherousCurrent } },
        { 7, new List<CalamityType> { CalamityType.TreacherousCurrent } },
        { 8, new List<CalamityType> { CalamityType.TreacherousCurrent, CalamityType.Waterspout, CalamityType.Whirlpool } },
        { 9, new List<CalamityType> { CalamityType.TreacherousCurrent, CalamityType.Waterspout } },
    };

    private Dictionary<int, Sprite> levelBackgroundsEasy;
    private Dictionary<int, Sprite> levelBackgroundsMedium;
    private Dictionary<int, Sprite> levelBackgroundsHard;
    private Dictionary<int, GameObject> easyLevelWaters;
    private Dictionary<int, GameObject> moderateLevelWaters;
    private Dictionary<int, GameObject> hardLevelWaters;

    // Initialize scene with appropriate background and calamity rules
    private void Start()
    {
        InitializeBackground();
        InitializeCalamities();
        ActivateWaterForLevel();
    }

    private void Awake()
    {
        // Initialize the dictionary in Awake or Start
        levelBackgroundsEasy = new Dictionary<int, Sprite>
        {
            { 1, beachImage },
            { 2, beachImage },
            { 3, beachImage },
            { 4, treasureCaveImage },
            { 5, treasureCaveImage },
            { 6, atollImage },
            { 7, sunkenCityImage },
            { 8, sunkenCityImage },
            { 9, sunkenCityImage },
            { 10, whirlIslandImage }
        };

        levelBackgroundsMedium = new Dictionary<int, Sprite>
        {
            { 1, volcanicIslandImage },
            { 2, volcanicIslandImage },
            { 3, wastelandImage },
            { 4, wastelandImage },
            { 5, wastelandImage },
            { 6, lighthouseImage },
            { 7, lighthouseImage },
            { 8, whirlIslandImage },
            { 9, whirlIslandImage },
            { 10, whirlIslandImage }
        };
        levelBackgroundsHard = new Dictionary<int, Sprite>
        {
            { 1, stormyOceanImage },
            { 2, stormyOceanImage },
            { 3, cloudyAreaImage },
            { 4, cloudyAreaImage },
            { 5, murkySeasImage },
            { 6, murkySeasImage },
            { 7, murkySeasImage },
            { 8, stormyOceanImage },
            { 9, lighthouseImage },
            { 10, wastelandImage }
        };
        // Initialize water mappings for easy levels
        easyLevelWaters = new Dictionary<int, GameObject>
        {
            { 1, beachWater },
            { 2, beachWater },
            { 3, beachWater },
            { 4, treasureCaveWater },
            { 5, treasureCaveWater },
            { 6, atollWater },
            { 7, sunkenCityWater },
            { 8, sunkenCityWater },
            { 9, sunkenCityWater },
            { 10, whirlIslandWater }
        };

        // Initialize water mappings for moderate levels
        moderateLevelWaters = new Dictionary<int, GameObject>
        {
            { 1, volcanicIslandWater },
            { 2, volcanicIslandWater },
            { 3, wastelandWater },
            { 4, wastelandWater },
            { 5, wastelandWater },
            { 6, lighthouseWater },
            { 7, lighthouseWater },
            { 8, whirlIslandWater },
            { 9, whirlIslandWater },
            { 10, murkySeasWater }
        };

        // Initialize water mappings for hard levels
        hardLevelWaters = new Dictionary<int, GameObject>
        {
            { 1, stormyOceanWater },
            { 2, stormyOceanWater },
            { 3, cloudyAreaWater },
            { 4, cloudyAreaWater },
            { 5, murkySeasWater },
            { 6, murkySeasWater },
            { 7, murkySeasWater },
            { 8, stormyOceanWater },
            { 9, lighthouseWater },
            { 10, wastelandWater }
        };
    }

    public void InitializeBackground()
    {
        int level = GameManager.instance.currentAILevel;
        GameMode mode = GameModeManager.instance.currentGameMode;

        Sprite backgroundImage = null;

        if (mode == GameMode.PlayerVsEnvironment && levelBackgroundsEasy.ContainsKey(level))
        {
            backgroundImage = levelBackgroundsEasy[level];
        }
        else if (mode == GameMode.PlayerVsEnvironmentMedium && levelBackgroundsMedium.ContainsKey(level))
        {
            backgroundImage = levelBackgroundsMedium[level];
        }
        else if (mode == GameMode.PlayerVsEnvironmentHard && levelBackgroundsHard.ContainsKey(level))
        {
            backgroundImage = levelBackgroundsHard[level];
        }
        if (backgroundImage != null)
        {
            background.sprite = backgroundImage;
        }
        else
        {
            Debug.LogWarning("No background set for this level and difficulty.");
        }
    }

    public void InitializeCalamities()
    {
        int level = GameManager.instance.currentAILevel;
        GameMode mode = GameModeManager.instance.currentGameMode;

        if (mode == GameMode.PlayerVsEnvironment && calamityByLevelEasy.ContainsKey(level))
        {
            allowedCalamities = calamityByLevelEasy[level];
        }
        else if (mode == GameMode.PlayerVsEnvironmentMedium && calamityByLevelMedium.ContainsKey(level))
        {
            allowedCalamities = calamityByLevelMedium[level];
        }
        else if (mode == GameMode.PlayerVsEnvironmentHard && calamityByLevelHard.ContainsKey(level))
        {
            allowedCalamities = calamityByLevelHard[level];
        }
        else if (mode == GameMode.PlayerVsPlayer)
        {
            // Allow all calamities for PlayerVsPlayer mode
            allowedCalamities = new List<CalamityType>
        {
            CalamityType.Reef,
            CalamityType.Whirlpool,
            CalamityType.TreacherousCurrent,
            CalamityType.PirateHideout,
            CalamityType.Waterspout
        };
        }
        else
        {
            allowedCalamities = new List<CalamityType> { CalamityType.None };
        }
    }

    private List<CalamityType> allowedCalamities;

    private void SpawnRandomCalamity()
    {
        if (allowedCalamities.Contains(CalamityType.None))
        {
            Debug.Log("No calamities to spawn for this level.");
            return;
        }

        CalamityType selectedCalamity = allowedCalamities[Random.Range(0, allowedCalamities.Count)];
        switch (selectedCalamity)
        {
            case CalamityType.Reef:
                shipboard.SpawnRandomReef();
                break;
            case CalamityType.Whirlpool:
                shipboard.SpawnRandomWhirlpool();
                break;
            case CalamityType.TreacherousCurrent:
                shipboard.SpawnRandomTreacherousCurrent();
                break;
            case CalamityType.PirateHideout:
                shipboard.SpawnRandomPirateHideout();
                break;
            case CalamityType.Waterspout:
                shipboard.SpawnRandomWaterspout();
                break;
        }
        Debug.Log($"Spawned calamity: {selectedCalamity}");
    }

    private void ActivateWaterForLevel()
    {
        int level = GameManager.instance.currentAILevel;
        GameMode mode = GameModeManager.instance.currentGameMode;

        Dictionary<int, GameObject> currentWaterMapping = null;

        // Select the correct mapping based on the game mode
        if (mode == GameMode.PlayerVsEnvironment)
        {
            currentWaterMapping = easyLevelWaters;
        }
        else if (mode == GameMode.PlayerVsEnvironmentMedium)
        {
            currentWaterMapping = moderateLevelWaters;
        }
        else if (mode == GameMode.PlayerVsEnvironmentHard)
        {
            currentWaterMapping = hardLevelWaters;
        }

        // Handle PlayerVsPlayer mode with a default water
        if (mode == GameMode.PlayerVsPlayer)
        {
            // Deactivate all other waters
            foreach (var water in easyLevelWaters.Values)
            {
                water.SetActive(false);
            }
            foreach (var water in moderateLevelWaters.Values)
            {
                water.SetActive(false);
            }
            foreach (var water in hardLevelWaters.Values)
            {
                water.SetActive(false);
            }

            // Activate the default water for PlayerVsPlayer
            sunkenCityWater.SetActive(true);
            Debug.Log("PlayerVsPlayer mode: Default sunken city water activated.");
            return; // Exit early since PlayerVsPlayer doesn't use mappings
        }

        // Ensure mapping exists and deactivate all waters for other modes
        if (currentWaterMapping != null)
        {
            foreach (var water in currentWaterMapping.Values)
            {
                water.SetActive(false);
            }

            // Activate water for the current level
            if (currentWaterMapping.ContainsKey(level))
            {
                currentWaterMapping[level].SetActive(true);
            }
            else
            {
                Debug.LogWarning($"No water GameObject mapped for level {level} in mode {mode}");
            }
        }
        else
        {
            Debug.LogWarning("No water mapping available for the current game mode.");
        }
    }

}
