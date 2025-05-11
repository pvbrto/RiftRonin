using UnityEngine;

public class InfoTrigger : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private string infoText;
    [SerializeField] private InfoUI infoUI; // Referência direta ao InfoUI
    
    private bool isPlayerInRange = false;

    private void Start()
    {
        if (infoUI == null)
        {
            Debug.LogError("InfoUI não foi atribuído no Inspector!");
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            infoUI.ShowInfo(infoText);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            infoUI.HideInfo();
        }
    }
} 