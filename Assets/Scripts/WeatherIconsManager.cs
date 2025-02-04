using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherIconsManager : MonoBehaviour
{
    // Enum to represent the weather types
    public enum WeatherType
    {
        Sunny,
        Cloudy,
        Rainy,
        Windy,
        Stormy,
        Snowy,
        Foggy,
        Hail
    }

    // Dictionaries to store icons for each weather type
    private Dictionary<WeatherType, int> weatherTypeToIndex = new Dictionary<WeatherType, int>();


    // Array of weather icons to assign
    public Sprite[] defaultWeatherSprite;
    public Sprite[] nextWeatherSprite;
    public Sprite[] currentWeatherSprite;

    public Image[] weatherIcons;

    public void InitializeWeatherIcons()
    {
        // Map WeatherType enum to indices in the array
        for (int i = 0; i < weatherIcons.Length; i++)
        {
            weatherTypeToIndex[(WeatherType)i] = i;
            weatherIcons[i].sprite = defaultWeatherSprite[i]; // Initialize with default sprites
        }
    }

    public void SetWeatherIcons(WeatherType current, WeatherType? next)
    {
        // First, reset all icons to default sprites
        for (int i = 0; i < weatherIcons.Length; i++)
        {
            weatherIcons[i].sprite = defaultWeatherSprite[i];
        }

        // Update the icon for the current weather
        int currentIndex = weatherTypeToIndex[current];
        weatherIcons[currentIndex].sprite = currentWeatherSprite[currentIndex];

        // Update the icon for the next weather, if it exists
        if (next.HasValue)
        {
            int nextIndex = weatherTypeToIndex[next.Value];
            weatherIcons[nextIndex].sprite = nextWeatherSprite[nextIndex];
        }
    }
}
