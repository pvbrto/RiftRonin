using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private GameObject boss;
    [SerializeField] private bool activateImmediately = false;
    [SerializeField] private AudioClip bossEntrySound;

    private bool isTriggered = false;

    private void Start()
    {
        // Verificar se o Boss está atribuído
        if (boss == null)
        {
            Debug.LogError("Boss não atribuído no BossTrigger!");
            return;
        }

        // Desativar o Boss inicialmente, a menos que esteja configurado para ativar imediatamente
        // if (!activateImmediately)
        // {
        //     boss.SetActive(false);
        // }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar se já foi acionado ou se o Boss não está atribuído
        if (isTriggered || boss == null)
        {
            return;
        }

        // Verificar se é o jogador
        if (collision.CompareTag("Player"))
        {
            isTriggered = true;
            
            // Iniciar a sequência do Boss
            StartCoroutine(BossEntry());
        }
    }

    private IEnumerator BossEntry()
    {
        // Tocar som de entrada do Boss, se disponível
        if (bossEntrySound != null)
        {
            AudioSource.PlayClipAtPoint(bossEntrySound, transform.position);
        }

        // Opcional: Criar uma pausa dramática
        yield return new WaitForSeconds(1.0f);

        // Ativar o Boss
        boss.SetActive(true);

        // Opcional: Shake na câmera para efeito dramático
        if (CameraControl.instance != null)
        {
            CameraControl.instance.ShakeCamera(0.5f);
        }

        // Opcional: Mudar música para a música do Boss
        // GameManager.instance.PlayBossMusic();

        // Destruir o trigger após ativar o Boss
        Destroy(gameObject, 2.0f);
    }
}