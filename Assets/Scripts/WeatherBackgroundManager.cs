using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherBackgroundManager : MonoBehaviour
{
    public RectTransform canvasRectTransform; // Assign your canvas RectTransform
    public GameObject[] cloudPrefabs; // Assign your cloud prefabs here
    public GameObject sunnyPrefab;
    public GameObject[] lightningZapPrefabs;
    public GameObject snowflakePrefab;
    public float cloudSpeed = 20f; // Speed of cloud movement
    public float lightningSpeed = 50f; // Speed of lightning movement


    private Vector2 canvasSize;
    private int activeClouds = 0; // Tracks the number of active clouds
    private const int maxClouds = 10; // Maximum number of clouds allowed

    private int activeLightning = 0; // Tracks the number of active lightning strikes
    private const int maxLightning = 5; // Maximum number of lightning strikes allowed

    private int activeSnowflakes = 0; // Tracks the number of active snowflakes
    private const int maxSnowflakes = 20; // Maximum number of snowflakes allowed

    private GameObject sunnyBackgroundInstance; // Tracks the sunny background instance
    private WeatherIconsManager.WeatherType currentWeather; // Tracks the current weather type

    void Start()
    {
        canvasSize = canvasRectTransform.sizeDelta;
        StartCoroutine(CloudManager());
        StartCoroutine(LightningManager());
        StartCoroutine(SnowflakeManager());
    }

    IEnumerator SnowflakeManager()
    {
        while (true)
        {
            if (currentWeather == WeatherIconsManager.WeatherType.Snowy && activeSnowflakes < maxSnowflakes)
            {
                SpawnSnowflake();
            }
            yield return new WaitForSeconds(Random.Range(0.5f, 2f)); // Adjust spawn rate for snowflakes
        }
    }

    void SpawnSnowflake()
    {
        // Choose a random snowflake prefab
        GameObject snowflakeInstance = Instantiate(snowflakePrefab, canvasRectTransform);

        RectTransform snowflakeRectTransform = snowflakeInstance.GetComponent<RectTransform>();

        // Set initial position at a random x coordinate, starting from the top of the screen
        float startX = Random.Range(-canvasSize.x / 2, canvasSize.x / 2);
        float startY = canvasSize.y / 2 + snowflakeRectTransform.rect.height; // Above the visible canvas
        snowflakeRectTransform.anchoredPosition = new Vector2(startX, startY);

        activeSnowflakes++;

        // Move snowflake downward
        StartCoroutine(MoveSnowflake(snowflakeRectTransform));
    }

    IEnumerator MoveSnowflake(RectTransform snowflakeRectTransform)
    {
        Vector2 startPosition = snowflakeRectTransform.anchoredPosition;
        Vector2 endPosition = new Vector2(startPosition.x, -canvasSize.y / 2 - snowflakeRectTransform.rect.height);

        while (snowflakeRectTransform.anchoredPosition.y > endPosition.y)
        {
            // Move towards the target position
            snowflakeRectTransform.anchoredPosition = Vector2.MoveTowards(snowflakeRectTransform.anchoredPosition, endPosition, cloudSpeed * Time.deltaTime);
            yield return null;
        }

        // Destroy the snowflake when it exits the screen
        Destroy(snowflakeRectTransform.gameObject);

        activeSnowflakes--;
    }

    void DestroyAllSnowflakes()
    {
        // Destroy all active snowflakes
        foreach (Transform child in canvasRectTransform)
        {
            if (child.CompareTag("Snowflake")) // Tag snowflakes with "Snowflake"
            {
                Destroy(child.gameObject);
                activeSnowflakes--;
            }
        }
        activeSnowflakes = 0; // Reset active snowflake count
    }

    IEnumerator CloudManager()
    {
        while (true)
        {
            if (currentWeather == WeatherIconsManager.WeatherType.Cloudy && activeClouds < maxClouds)
            {
                SpawnCloud();
            }
            yield return new WaitForSeconds(Random.Range(5f, 15f)); // Adjust spawn rate
        }
    }
    IEnumerator LightningManager()
    {
        while (true)
        {
            if (currentWeather == WeatherIconsManager.WeatherType.Stormy && activeLightning < maxLightning)
            {
                SpawnLightning();
            }
            yield return new WaitForSeconds(Random.Range(1f, 5f)); // Adjust spawn rate
        }
    }

    public void UpdateWeather(WeatherIconsManager.WeatherType newWeather)
    {
        // Check if the weather has changed
        if (newWeather != currentWeather)
        {
            currentWeather = newWeather;

            // Handle sunny weather
            if (currentWeather == WeatherIconsManager.WeatherType.Sunny)
            {
                SpawnSunnyBackground();
                DestroyAllClouds();
            }
            else
            {
                DestroySunnyBackground();
            }

            // Handle cloudy weather
            if (currentWeather != WeatherIconsManager.WeatherType.Cloudy)
            {
                DestroyAllClouds();
            }

            // Handle stormy weather
            if (currentWeather != WeatherIconsManager.WeatherType.Stormy)
            {
                DestroyAllLightning(); // Destroy lightning if no longer stormy
            }

            // Handle snowy weather
            if (currentWeather != WeatherIconsManager.WeatherType.Snowy)
            {
                DestroyAllSnowflakes(); // Destroy snowflakes if no longer snowy
            }
        }
    }

    void SpawnCloud()
    {
        // Choose a random prefab
        GameObject cloudPrefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];
        GameObject cloudInstance = Instantiate(cloudPrefab, canvasRectTransform);

        RectTransform cloudRectTransform = cloudInstance.GetComponent<RectTransform>();

        // Determine spawn side (left or right)
        bool spawnOnLeft = Random.value > 0.5f;
        float startX = spawnOnLeft ? -canvasSize.x / 2 - cloudRectTransform.rect.width : canvasSize.x / 2 + cloudRectTransform.rect.width;
        float targetX = spawnOnLeft ? canvasSize.x / 2 + cloudRectTransform.rect.width : -canvasSize.x / 2 - cloudRectTransform.rect.width;

        // Randomize spawn height within canvas
        float startY = Random.Range(-canvasSize.y / 2, canvasSize.y / 2);

        // Set cloud's initial position
        cloudRectTransform.anchoredPosition = new Vector2(startX, startY);

        activeClouds++;

        // Move cloud
        StartCoroutine(MoveCloud(cloudRectTransform, targetX));
    }

    IEnumerator MoveCloud(RectTransform cloudRectTransform, float targetX)
    {
        Vector2 startPosition = cloudRectTransform.anchoredPosition;
        Vector2 endPosition = new Vector2(targetX, startPosition.y);

        
        if (cloudRectTransform != null)
            while (Vector2.Distance(cloudRectTransform.anchoredPosition, endPosition) > 0.1f)
            {
                // Move towards the target position
                cloudRectTransform.anchoredPosition = Vector2.MoveTowards(cloudRectTransform.anchoredPosition, endPosition, cloudSpeed * Time.deltaTime);
                yield return null;
            }

        // Destroy the cloud when it exits the screen
        Destroy(cloudRectTransform.gameObject);

        activeClouds--;
    }

    void DestroyAllClouds()
    {
        // Destroy all active clouds
        foreach (Transform child in canvasRectTransform)
        {
            if (child.CompareTag("Cloud")) // Tag clouds with "Cloud"
            {
                Destroy(child.gameObject);
                activeClouds--;
            }
        }
        activeClouds = 0;
    }

    void SpawnLightning()
    {
        // Choose a random lightning prefab
        GameObject lightningPrefab = lightningZapPrefabs[Random.Range(0, lightningZapPrefabs.Length)];
        GameObject lightningInstance = Instantiate(lightningPrefab, canvasRectTransform);

        RectTransform lightningRectTransform = lightningInstance.GetComponent<RectTransform>();

        // Set initial position at a random x coordinate, starting from the top of the screen
        float startX = Random.Range(-canvasSize.x / 2, canvasSize.x / 2);
        float startY = canvasSize.y / 2 + lightningRectTransform.rect.height; // Above the visible canvas
        lightningRectTransform.anchoredPosition = new Vector2(startX, startY);

        // Start with height 0, growing downward
        lightningRectTransform.sizeDelta = new Vector2(lightningRectTransform.sizeDelta.x, 0);

        activeLightning++;

        // Grow lightning downward
        StartCoroutine(GrowLightning(lightningRectTransform));
    }

    IEnumerator GrowLightning(RectTransform lightningRectTransform)
    {
        float targetHeight = canvasSize.y; // Target height
        float bottomY = -canvasSize.y / 2; // Bottom of the screen
        Vector2 sizeDelta = lightningRectTransform.sizeDelta;
        float originalWidth = sizeDelta.x;

        while (lightningRectTransform.anchoredPosition.y > bottomY)
        {
            // Determine a random segment length
            float segmentHeight = Random.Range(20f, 100f);
            sizeDelta.y += segmentHeight;

            // Adjust the lightning's size and position for this segment
            lightningRectTransform.sizeDelta = sizeDelta;
            lightningRectTransform.anchoredPosition += new Vector2(0, -segmentHeight);

            // Add width pulsing effect
            lightningRectTransform.sizeDelta = new Vector2(originalWidth * 1.2f, sizeDelta.y); // Temporarily widen
            yield return new WaitForSeconds(0.05f); // Brief flash duration
            lightningRectTransform.sizeDelta = new Vector2(originalWidth, sizeDelta.y); // Reset width

            // Add a brief snappy delay
            yield return new WaitForSeconds(Random.Range(0.02f, 0.1f));
        }

        // Destroy the lightning once it reaches the bottom
        Destroy(lightningRectTransform.gameObject);

        activeLightning--;
    }

    void SpawnSunnyBackground()
    {
        if (sunnyBackgroundInstance == null)
        {
            sunnyBackgroundInstance = Instantiate(sunnyPrefab, canvasRectTransform);
        }
    }

    void DestroySunnyBackground()
    {
        if (sunnyBackgroundInstance != null)
        {
            Destroy(sunnyBackgroundInstance);
            sunnyBackgroundInstance = null;
        }
    }
    void DestroyAllLightning()
    {
        // Destroy all active lightning strikes
        foreach (Transform child in canvasRectTransform)
        {
            if (child.CompareTag("Lightning")) // Tag lightning prefabs with "Lightning"
            {
                Destroy(child.gameObject);
                activeLightning--;
            }
        }
        activeLightning = 0; // Reset active lightning count
    }

}
