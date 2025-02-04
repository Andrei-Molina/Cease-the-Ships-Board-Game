using System.Collections.Generic;
using UnityEngine;

public class MarkovChainWeather : MonoBehaviour
{
    private MersenneTwister mt;
    public WeatherIconsManager weatherIconsManager;
    public WeatherBackgroundManager weatherBackgroundManager;

    private readonly string[] weatherConditions = {
        "Sunny",
        "Cloudy",
        "Rainy",
        "Windy",
        "Stormy",
        "Snowy",
        "Foggy",
        "Hail"
    };

    private readonly Dictionary<string, Dictionary<string, float>> transitionMatrix = new Dictionary<string, Dictionary<string, float>>
{
    { "Sunny", new Dictionary<string, float> { { "Cloudy", 0.6f }, { "Windy", 0.3f }, { "Rainy", 0.1f } } },
    { "Cloudy", new Dictionary<string, float> { { "Sunny", 0.3f }, { "Rainy", 0.4f }, { "Foggy", 0.2f }, { "Snowy", 0.1f } } },
    { "Rainy", new Dictionary<string, float> { { "Cloudy", 0.5f }, { "Windy", 0.3f }, { "Stormy", 0.2f } } },
    { "Snowy", new Dictionary<string, float> { { "Cloudy", 0.7f }, { "Sunny", 0.2f }, { "Windy", 0.1f } } },
    { "Foggy", new Dictionary<string, float> { { "Sunny", 0.5f }, { "Cloudy", 0.4f }, { "Windy", 0.1f } } },
    { "Hail", new Dictionary<string, float> { { "Stormy", 0.8f }, { "Windy", 0.2f } } },
    { "Windy", new Dictionary<string, float> { { "Cloudy", 0.4f }, { "Rainy", 0.3f }, { "Stormy", 0.2f }, { "Snowy", 0.1f } } },
    { "Stormy", new Dictionary<string, float> { { "Windy", 0.5f }, { "Cloudy", 0.3f }, { "Rainy", 0.1f }, { "Snowy", 0.1f } } }
};

    private string currentWeather;
    private List<string> predictedWeatherSequence = new List<string>();
    private int currentWeatherIndex;
    private int lastWeatherUpdateTurn = 0; // Tracks the last turn when weather was updated

    void Start()
    {
        weatherIconsManager.InitializeWeatherIcons();
        mt = new MersenneTwister((uint)System.DateTime.Now.Ticks); // Seed with current time
        PredictWeatherSequence(10); // Predict 10 weather conditions
        InitializeWeather();
    }

    private void Update()
    {
        // Check if it's time to change weather and ensure it only runs once per 3 turns
        if (GameManager.instance.turns > 0 &&
            GameManager.instance.turns % GameManager.instance.weatherTurns == 0 &&
            GameManager.instance.turns != lastWeatherUpdateTurn) // Ensure it doesn't run twice in the same turn
        {
            lastWeatherUpdateTurn = GameManager.instance.turns; // Update the last update turn
            OnWeatherChange();
        }
    }

    void InitializeWeather()
    {
        currentWeatherIndex = 0;
        UpdateWeatherSprites(); // Initialize the first weather setup
    }

    void PredictWeatherSequence(int count)
    {
        // Start from the last weather in the sequence if it exists
        string weather = predictedWeatherSequence.Count > 0
            ? predictedWeatherSequence[predictedWeatherSequence.Count - 1]
            : GetRandomWeather();

        for (int i = 0; i < count; i++)
        {
            weather = GetNextWeather(weather);
            predictedWeatherSequence.Add(weather);
        }
    }

    string GetRandomWeather()
    {
        int index = mt.Next(weatherConditions.Length);
        return weatherConditions[index];
    }

    string GetNextWeather(string currentWeather)
    {
        if (!transitionMatrix.ContainsKey(currentWeather))
        {
            return currentWeather; // No transitions defined, return the current weather
        }

        var probabilities = transitionMatrix[currentWeather];
        float rand = mt.GenrandInt32() / (float)uint.MaxValue; // Random value between 0 and 1
        float cumulativeProbability = 0.0f;

        foreach (var kvp in probabilities)
        {
            cumulativeProbability += kvp.Value;
            if (rand < cumulativeProbability)
                return kvp.Key;
        }

        return currentWeather; // Fallback, should not usually reach here
    }

    void UpdateWeatherSprites()
    {
        if (currentWeatherIndex >= predictedWeatherSequence.Count)
        {
            PredictWeatherSequence(10); // Extend the sequence by 10 more predictions
        }

        if (weatherIconsManager == null || currentWeatherIndex >= predictedWeatherSequence.Count)
            return;

        string currentWeather = predictedWeatherSequence[currentWeatherIndex];
        string nextWeather = (currentWeatherIndex + 1 < predictedWeatherSequence.Count)
            ? predictedWeatherSequence[currentWeatherIndex + 1]
            : null;

        // Convert weather strings to WeatherType enum
        WeatherIconsManager.WeatherType currentWeatherType = ParseWeatherType(currentWeather);
        WeatherIconsManager.WeatherType? nextWeatherType = nextWeather != null
            ? ParseWeatherType(nextWeather)
            : null;

        weatherIconsManager.SetWeatherIcons(currentWeatherType, nextWeatherType);

        // Notify WeatherBackgroundManager of weather change
        if (weatherBackgroundManager != null)
        {
            weatherBackgroundManager.UpdateWeather(currentWeatherType);
        }

        currentWeatherIndex++;
    }

    WeatherIconsManager.WeatherType ParseWeatherType(string weather)
    {
        if (System.Enum.TryParse(weather, out WeatherIconsManager.WeatherType weatherType))
            return weatherType;
        Debug.LogWarning($"Weather '{weather}' could not be parsed to WeatherType.");
        return WeatherIconsManager.WeatherType.Sunny; // Fallback to default
    }

    public void OnWeatherChange()
    {
        UpdateWeatherSprites();
    }
}
