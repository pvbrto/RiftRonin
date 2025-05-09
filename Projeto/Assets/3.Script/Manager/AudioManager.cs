using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Main Music")]
    [SerializeField] private AudioClip mainMusic;

    [Header("Weather Sound Effects")]
    [SerializeField] private AudioClip ensolaradoSound;
    [SerializeField] private AudioClip invernoSound;
    [SerializeField] private AudioClip tempestadeSound;
    [SerializeField] private AudioClip noiteSound;

    [Header("Player Sound Effects")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip jumpSound;

    [Header("Enemy Sound Effects")]
    [SerializeField] private AudioClip enemyDeathSound;

    [Header("Audio Settings")]
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource weatherSource;
    private AudioSource sfxSource;
    private AudioClip currentWeatherSound;
    private Coroutine fadeCoroutine;
    private bool isChangingWeather = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Configurar fonte de áudio para música principal
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = musicVolume;

            // Configurar fonte de áudio para efeitos de clima
            weatherSource = gameObject.AddComponent<AudioSource>();
            weatherSource.loop = true;
            weatherSource.volume = sfxVolume;

            // Configurar fonte de áudio para efeitos do player
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = sfxVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Tocar música principal ao iniciar
        if (mainMusic != null)
        {
            musicSource.clip = mainMusic;
            musicSource.Play();
        }

        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.OnWeatherChanged += HandleWeatherChange;
        }
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
        AudioClip newWeatherSound = GetWeatherSound(newWeather);
        if (newWeatherSound != null && newWeatherSound != currentWeatherSound)
        {
            if (isChangingWeather)
            {
                StopCoroutine(fadeCoroutine);
                weatherSource.Stop();
            }
            PlayWeatherSound(newWeatherSound);
        }
    }

    private AudioClip GetWeatherSound(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Ensolarado:
                return ensolaradoSound;
            case WeatherType.Inverno:
                return invernoSound;
            case WeatherType.Tempestade:
                return tempestadeSound;
            case WeatherType.Noite:
                return noiteSound;
            default:
                return ensolaradoSound;
        }
    }

    private void PlayWeatherSound(AudioClip weatherSound)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeWeatherSound(weatherSound));
    }

    private System.Collections.IEnumerator FadeWeatherSound(AudioClip newWeatherSound)
    {
        isChangingWeather = true;

        // Fade out do som atual
        float startVolume = weatherSource.volume;
        float timer = 0;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            weatherSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeTime);
            yield return null;
        }

        // Trocar o som
        weatherSource.clip = newWeatherSound;
        currentWeatherSound = newWeatherSound;
        weatherSource.Play();

        // Fade in do novo som
        timer = 0;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            weatherSource.volume = Mathf.Lerp(0, sfxVolume, timer / fadeTime);
            yield return null;
        }

        weatherSource.volume = sfxVolume;
        isChangingWeather = false;
    }

    public void PlayAttackSound()
    {
        if (attackSound != null)
        {
            sfxSource.PlayOneShot(attackSound);
        }
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            sfxSource.PlayOneShot(jumpSound);
        }
    }

    public void PlayEnemyDeathSound()
    {
        if (enemyDeathSound != null)
        {
            sfxSource.PlayOneShot(enemyDeathSound);
        }
    }
} 