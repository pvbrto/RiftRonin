using UnityEngine;

public class WeatherEffectsManager : MonoBehaviour
{
    public static WeatherEffectsManager Instance { get; private set; }

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem snowParticles;
    [SerializeField] private ParticleSystem rainParticles;

    private ParticleSystem currentActiveParticles;

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

    private void Start()
    {
        // Inscreve-se para receber atualizações de clima
        WeatherManager.Instance.OnWeatherChanged += HandleWeatherChange;
    }

    private void OnDestroy()
    {
        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.OnWeatherChanged -= HandleWeatherChange;
        }
    }

    private void HandleWeatherChange(WeatherType newWeather)
    {
        StopAllParticles();

        switch (newWeather)
        {
            case WeatherType.Inverno:
                if (snowParticles != null)
                {
                    snowParticles.Play();
                    currentActiveParticles = snowParticles;
                }
                break;

            case WeatherType.Tempestade:
                if (rainParticles != null)
                {
                    rainParticles.Play();
                    currentActiveParticles = rainParticles;
                }
                break;

            default:
                // Para outros tipos de clima, não há partículas
                break;
        }
    }

    private void StopAllParticles()
    {
        if (currentActiveParticles != null)
        {
            currentActiveParticles.Stop();
        }
    }
} 