using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public string text;
    public Sprite characterSprite;
    public bool isPlayer;
}

public class DialogueSystem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float textSpeed = 0.05f;

    [Header("Dialogue Settings")]
    [SerializeField] private List<DialogueLine> dialogueLines;
    [SerializeField] private KeyCode continueKey = KeyCode.Space;

    private CanvasGroup canvasGroup;
    private int currentLine = 0;
    private bool isShowing = false;
    private bool isTyping = false;
    private string currentText = "";

    private void Awake()
    {
        canvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = dialoguePanel.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 0f;
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isShowing) return;

        if (Input.GetKeyDown(continueKey))
        {
            if (isTyping)
            {
                // Skip typing animation
                dialogueText.text = currentText;
                isTyping = false;
            }
            else
            {
                // Move to next line
                currentLine++;
                if (currentLine >= dialogueLines.Count)
                {
                    HideDialogue();
                }
                else
                {
                    ShowNextLine();
                }
            }
        }
    }

    public void StartDialogue(List<DialogueLine> lines)
    {
        dialogueLines = lines;
        currentLine = 0;
        dialoguePanel.SetActive(true);
        isShowing = true;
        StopAllCoroutines();
        StartCoroutine(FadeIn());
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (currentLine >= dialogueLines.Count) return;

        DialogueLine line = dialogueLines[currentLine];
        currentText = line.text;
        characterImage.sprite = line.characterSprite;
        characterImage.gameObject.SetActive(line.characterSprite != null);
        
        // Start typing animation
        dialogueText.text = "";
        isTyping = true;
        StartCoroutine(TypeText());
    }

    private System.Collections.IEnumerator TypeText()
    {
        dialogueText.text = "";
        foreach (char c in currentText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    public void HideDialogue()
    {
        isShowing = false;
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        dialoguePanel.SetActive(false);
    }
} 