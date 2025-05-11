using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private List<DialogueLine> dialogueSequence;
    [SerializeField] private Sprite npcSprite; // Sprite do NPC
    [SerializeField] private Sprite playerSprite; // Sprite do jogador
    
    private bool isPlayerInRange = false;

    private void Start()
    {
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem não foi atribuído no Inspector!");
        }

        // Exemplo de como criar uma sequência de diálogo
        if (dialogueSequence.Count == 0)
        {
            dialogueSequence = new List<DialogueLine>
            {
                new DialogueLine 
                { 
                    text = "Olá, aventureiro! Bem-vindo à nossa cidade.", 
                    characterSprite = npcSprite,
                    isPlayer = false 
                },
                new DialogueLine 
                { 
                    text = "Olá! Obrigado pelo acolhimento.", 
                    characterSprite = playerSprite,
                    isPlayer = true 
                },
                new DialogueLine 
                { 
                    text = "Estamos precisando de ajuda com algumas missões. Você poderia nos ajudar?", 
                    characterSprite = npcSprite,
                    isPlayer = false 
                },
                new DialogueLine 
                { 
                    text = "Claro! Estou sempre disposto a ajudar.", 
                    characterSprite = playerSprite,
                    isPlayer = true 
                }
            };
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            dialogueSystem.StartDialogue(dialogueSequence);
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
        }
    }
} 