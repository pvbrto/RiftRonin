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

    // Evento para notificar mudanças de clima
    public event Action<WeatherType> OnWeatherChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetWeather(WeatherType newWeather)
    {
        currentWeather = newWeather;
        OnWeatherChanged?.Invoke(currentWeather);
        Debug.Log("Weather changed to: " + currentWeather);
    }
} 