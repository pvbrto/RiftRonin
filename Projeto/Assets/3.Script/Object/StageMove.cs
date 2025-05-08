using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMove : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private GameObject[] backgroundPrefabs;
    [SerializeField] private Transform backgroundParent;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    private void Start()
    {
        // Garantir que o BackgroundParent existe
        if (backgroundParent == null)
        {
            backgroundParent = GameObject.Find("BackgroundContainer")?.transform;
            if (backgroundParent == null)
            {
                Debug.LogError("BackgroundContainer não encontrado na cena!");
                return;
            }
        }

        // Inicializar o background baseado no clima atual
        if (WeatherManager.Instance != null)
        {
            WeatherManager.Instance.InitializeBackground(backgroundParent, backgroundPrefabs, spawnOffset);
        }
        else
        {
            Debug.LogError("WeatherManager não encontrado!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Colidiu com o player");  
            Debug.Log(SceneManager.GetActiveScene().name);
            if (SceneManager.GetActiveScene().name == "Inicio")
            {
                SceneManager.LoadScene("Cena1");
            }  
            if (SceneManager.GetActiveScene().name == "Cena1")
            {
                SceneManager.LoadScene("Cena2");
            }
        }
    }
}
