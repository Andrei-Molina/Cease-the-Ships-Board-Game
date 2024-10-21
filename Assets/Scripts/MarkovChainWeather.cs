using System.Collections.Generic;
using UnityEngine;

public class MarkovChainWeather : MonoBehaviour
{
    private MersenneTwister mt;
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
    { "Sunny", new Dictionary<string, float> { { "Cloudy", 0.6f }, { "Windy", 0.4f } } },
    { "Cloudy", new Dictionary<string, float> { { "Sunny", 0.3f }, { "Rainy", 0.4f }, { "Foggy", 0.3f } } },
    { "Rainy", new Dictionary<string, float> { { "Windy", 0.5f }, { "Stormy", 0.5f } } },
    { "Snowy", new Dictionary<string, float> { { "Cloudy", 0.6f }, { "Sunny", 0.4f } } },
    { "Foggy", new Dictionary<string, float> { { "Sunny", 0.5f }, { "Windy", 0.5f } } },
    { "Hail", new Dictionary<string, float> { { "Stormy", 0.7f }, { "Windy", 0.3f } } },
    { "Windy", new Dictionary<string, float> { { "Stormy", 0.4f }, { "Rainy", 0.4f }, { "Cloudy", 0.2f } } },
    { "Stormy", new Dictionary<string, float> { { "Windy", 0.4f }, { "Rainy", 0.3f }, { "Cloudy", 0.2f }, { "Foggy", 0.1f } } }
};

    private string currentWeather;

    void Start()
    {
        mt = new MersenneTwister((uint)System.DateTime.Now.Ticks); // Seed with current time
        currentWeather = GetRandomWeather(); // Start with a random weather condition
        SimulateWeather();
    }

    void SimulateWeather()
    {
        for (int i = 0; i < 10; i++) // Simulate 10 weather conditions
        {
            Debug.Log(currentWeather);
            currentWeather = GetNextWeather(currentWeather);
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
}
