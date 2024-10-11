using UnityEngine;

public class WeatherSimulation : MonoBehaviour
{
    private MersenneTwister mt;
    private string[] weatherConditions = {
        "Stormy",
        "Rainy",
        "Winter",
        "Cloudy",
        "Foggy",
        "Snowy",
        "Hail",
        "Windy",
        "Sunny"
    };

    void Start()
    {
        mt = new MersenneTwister((uint)System.DateTime.Now.Ticks); // Seed with current time
        SimulateWeather();
    }

    void SimulateWeather()
    {
        string weather = GetRandomWeather();
        Debug.Log(weather);
    }

    string GetRandomWeather()
    {
        int index = mt.Next(weatherConditions.Length);
        return weatherConditions[index];
    }
}
