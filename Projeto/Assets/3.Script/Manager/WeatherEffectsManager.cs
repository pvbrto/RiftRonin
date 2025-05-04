using UnityEngine;

public class WeatherEffectsManager : MonoBehaviour
{
    public static WeatherEffectsManager Instance { get; private set; }

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem snowParticles;
    [SerializeField] private ParticleSystem rainParticles;

    [Header("Configurações de Posição")]
    [SerializeField] private Vector3 particleSystemPosition = new Vector3(0, 10, 0);
    [SerializeField] private bool followCamera = true;
    [SerializeField] private bool useCustomPosition = false; // Se false, mantém a posição do prefab

    [Header("Configurações de Partículas")]
    [SerializeField] private int maxParticles = 1000;
    [SerializeField] private bool overrideMaxParticles = true;

    private ParticleSystem currentActiveParticles;
    private Camera mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Manter os sistemas de partículas como filhos deste manager
            if (snowParticles != null)
            {
                snowParticles.transform.parent = this.transform;
            }
            if (rainParticles != null)
            {
                rainParticles.transform.parent = this.transform;
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        mainCamera = Camera.main;
        InitializeParticleSystems();
    }

    private void Update()
    {
        if (followCamera && mainCamera != null && currentActiveParticles != null)
        {
            // Atualiza apenas a posição X e Z para seguir a câmera, mantendo a altura original
            Vector3 currentPos = currentActiveParticles.transform.position;
            Vector3 cameraPosition = mainCamera.transform.position;
            currentActiveParticles.transform.position = new Vector3(
                cameraPosition.x,
                useCustomPosition ? particleSystemPosition.y : currentPos.y,
                cameraPosition.z
            );
        }
    }

    private void InitializeParticleSystems()
    {
        if (snowParticles == null)
        {
            Debug.LogError("Snow Particles não foi atribuído no Inspector!");
            return;
        }

        // Configurações básicas mantendo o resto do prefab
        var mainModule = snowParticles.main;
        mainModule.playOnAwake = false;
        
        // Atualiza o número máximo de partículas se necessário
        if (overrideMaxParticles)
        {
            mainModule.maxParticles = maxParticles;
            Debug.Log($"Número máximo de partículas definido para: {maxParticles}");
        }

        // Define posição inicial apenas se useCustomPosition estiver ativado
        if (useCustomPosition)
        {
            snowParticles.transform.position = particleSystemPosition;
        }

        Debug.Log($"Sistema de partículas de neve inicializado. Posição: {snowParticles.transform.position}");
    }

    private void Start()
    {
        if (WeatherManager.Instance == null)
        {
            Debug.LogError("WeatherManager não encontrado na cena!");
            return;
        }
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

        Debug.Log("Loading Particles for " + newWeather);

        switch (newWeather)
        {
            case WeatherType.Inverno:
                Debug.Log("Inverno");
                if (snowParticles != null)
                {
                    Debug.Log("Tentando iniciar partículas de neve...");
                    
                    // Verificar estado atual
                    Debug.Log($"Estado atual - IsAlive: {snowParticles.IsAlive()}, IsEmitting: {snowParticles.isEmitting}");
                    
                    // Garantir que o sistema está ativo
                    snowParticles.gameObject.SetActive(true);
                    
                    // Define posição apenas se useCustomPosition estiver ativado
                    if (useCustomPosition)
                    {
                        snowParticles.transform.position = particleSystemPosition;
                    }
                    
                    // Limpar e reiniciar
                    snowParticles.Clear();
                    snowParticles.Play(true);
                    
                    // Verificar estado após iniciar
                    if (snowParticles.isPlaying)
                    {
                        var main = snowParticles.main;
                        Debug.Log("Snow Particles está em execução!");
                        Debug.Log($"Particles Count: {snowParticles.particleCount}");
                        Debug.Log($"Max Particles: {main.maxParticles}");
                        Debug.Log($"Posição atual: {snowParticles.transform.position}");
                    }
                    else
                    {
                        Debug.LogError("Snow Particles não iniciou corretamente!");
                        Debug.LogError($"GameObject Ativo: {snowParticles.gameObject.activeSelf}");
                    }
                    
                    currentActiveParticles = snowParticles;
                }
                break;

            case WeatherType.Tempestade:
                if (rainParticles != null)
                {
                    rainParticles.Clear();
                    rainParticles.Play();
                    currentActiveParticles = rainParticles;
                }
                break;

            default:
                break;
        }
    }

    private void StopAllParticles()
    {
        if (currentActiveParticles != null)
        {
            currentActiveParticles.Stop();
            currentActiveParticles.Clear();
        }
    }

    // Método para debug - pode ser chamado pelo Inspector
    public void ForcePlaySnow()
    {
        if (snowParticles != null)
        {
            Debug.Log("Forçando execução das partículas de neve");
            StopAllParticles();
            
            // Verificar e garantir que está ativo
            snowParticles.gameObject.SetActive(true);
            
            // Define posição apenas se useCustomPosition estiver ativado
            if (useCustomPosition)
            {
                snowParticles.transform.position = particleSystemPosition;
            }
            
            snowParticles.Clear();
            snowParticles.Play(true);
            
            var main = snowParticles.main;
            Debug.Log($"Status após ForcePlay - IsPlaying: {snowParticles.isPlaying}, ParticleCount: {snowParticles.particleCount}");
            Debug.Log($"Max Particles: {main.maxParticles}");
            Debug.Log($"Posição atual: {snowParticles.transform.position}");
        }
    }
} 