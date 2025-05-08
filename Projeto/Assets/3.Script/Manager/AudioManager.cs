using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music Clips")]
    [SerializeField] private AudioClip ensolaradoMusic;
    [SerializeField] private AudioClip invernoMusic;
    [SerializeField] private AudioClip tempestadeMusic;
    [SerializeField] private AudioClip noiteMusic;

    [Header("Audio Settings")]
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private float volume = 1f;

    private AudioSource audioSource;
    private AudioClip currentMusic;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.loop = true;
            audioSource.volume = volume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
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
        AudioClip newMusic = GetMusicForWeather(newWeather);
        if (newMusic != null && newMusic != currentMusic)
        {
            PlayMusic(newMusic);
        }
    }

    private AudioClip GetMusicForWeather(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Ensolarado:
                return ensolaradoMusic;
            case WeatherType.Inverno:
                return invernoMusic;
            case WeatherType.Tempestade:
                return tempestadeMusic;
            case WeatherType.Noite:
                return noiteMusic;
            default:
                return ensolaradoMusic;
        }
    }

    private void PlayMusic(AudioClip music)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeMusic(music));
    }

    private System.Collections.IEnumerator FadeMusic(AudioClip newMusic)
    {
        // Fade out
        float startVolume = audioSource.volume;
        float timer = 0;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeTime);
            yield return null;
        }

        // Change music
        audioSource.clip = newMusic;
        currentMusic = newMusic;
        audioSource.Play();

        // Fade in
        timer = 0;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, volume, timer / fadeTime);
            yield return null;
        }

        audioSource.volume = volume;
    }
} 