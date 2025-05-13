using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private BossController bossController; // Recebe diretamente o componente
    [SerializeField] private bool activateImmediately = false;
    [SerializeField] private AudioClip bossEntrySound;

    private bool isTriggered = false;

    private void Start()
    {
        if (bossController == null)
        {
            Debug.LogError("BossController não atribuído no BossTrigger!");
            return;
        }

        if (activateImmediately)
        {
            ActivateBoss();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger detectou: " + collision.gameObject.name);

        if (isTriggered || bossController == null)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player detectado! Ativando Boss");
            ActivateBoss();
        }
    }

    private void ActivateBoss()
    {
        isTriggered = true;
        
        Debug.Log("Método ActivateBoss chamado no BossTrigger");
        
        // Tocar som de entrada do Boss, se disponível
        if (bossEntrySound != null)
        {
            AudioSource.PlayClipAtPoint(bossEntrySound, transform.position);
        }
        
        // Ativar o Boss
        if (bossController != null)
        {
            Debug.Log("Chamando método ActivateBoss() no BossController");
            bossController.ActivateBoss();
        }
        else
        {
            Debug.LogError("BossController é null! Não foi possível ativar o boss.");
        }
        
        // Destruir o trigger após ativar o Boss
        Destroy(gameObject, 5.0f);
    }
}