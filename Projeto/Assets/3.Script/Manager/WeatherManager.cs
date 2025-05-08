using UnityEngine;
using System;

public enum WeatherType
{
    Ensolarado,
    Inverno,
    Tempestade,
    Noite
}

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    private WeatherType currentWeather;
    public WeatherType CurrentWeather => currentWeather;
    private bool isInitialized = false;

    // Evento para notificar mudanças de clima
    public event Action<WeatherType> OnWeatherChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Definir clima inicial como Ensolarado
            currentWeather = WeatherType.Ensolarado;
            isInitialized = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetWeather(WeatherType newWeather)
    {
        currentWeather = newWeather;
        isInitialized = true;
        OnWeatherChanged?.Invoke(currentWeather);
        Debug.Log("Weather changed to: " + currentWeather);
    }

    public void InitializeBackground(Transform backgroundParent, GameObject[] backgroundPrefabs, Vector3 spawnOffset)
    {
        if (backgroundPrefabs == null || backgroundPrefabs.Length == 0 || backgroundParent == null)
        {
            Debug.LogError("Background initialization failed: Missing required components");
            return;
        }

        // Se o clima não foi inicializado, definir como Ensolarado
        if (!isInitialized)
        {
            currentWeather = WeatherType.Ensolarado;
            isInitialized = true;
        }

        // Limpar background anterior
        foreach (Transform child in backgroundParent)
        {
            Destroy(child.gameObject);
        }

        // Criar novo background baseado no clima atual
        int weatherIndex = (int)currentWeather;
        if (weatherIndex >= backgroundPrefabs.Length)
        {
            Debug.LogWarning($"Weather index {weatherIndex} is out of range for background prefabs. Using default (0)");
            weatherIndex = 0;
        }

        GameObject selectedPrefab = backgroundPrefabs[weatherIndex];
        Vector3 spawnPosition = backgroundParent.position + spawnOffset;

        if (currentWeather == WeatherType.Inverno)
        {
            spawnPosition.y += 0.8f;
        }

        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity, backgroundParent);
        Debug.Log($"Background initialized for weather: {currentWeather}");
    }
} 