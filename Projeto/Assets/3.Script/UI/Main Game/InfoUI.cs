using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InfoUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject infoPanel;
    // [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private float fadeSpeed = 5f;

    [Header("Text Animation")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private string japaneseText = "こんにちは";

    private CanvasGroup canvasGroup;
    private bool isShowing = false;
    private string fullText;
    private Coroutine currentAnimation;

    private void Awake()
    {
        // Garante que temos um CanvasGroup
        canvasGroup = infoPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = infoPanel.AddComponent<CanvasGroup>();
        }
        
        // Inicia com o painel invisível
        canvasGroup.alpha = 0f;
        infoPanel.SetActive(false);
    }

    public void ShowInfo(string text)
    {
        // Guarda o texto completo e mostra o texto em japonês
        fullText = text;
        infoText.text = japaneseText;
        
        // Mostra o painel
        infoPanel.SetActive(true);
        isShowing = true;
        
        // Inicia o fade in e a animação do texto
        StopAllCoroutines();
        StartCoroutine(FadeIn());
        currentAnimation = StartCoroutine(AnimateText());
    }

    public void HideInfo()
    {
        if (!isShowing) return;
        
        isShowing = false;
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator AnimateText()
    {
        // Espera o fade in terminar
        yield return new WaitForSeconds(0.5f);

        // Para cada caractere do texto em português
        for (int i = 0; i < fullText.Length; i++)
        {
            // Cria o texto atual: caracteres em português até agora + caracteres em japonês restantes
            string portuguesePart = fullText.Substring(0, i + 1);
            string japanesePart = "";
            
            // Só adiciona a parte japonesa se ainda houver caracteres japoneses restantes
            if (i + 1 < japaneseText.Length)
            {
                japanesePart = japaneseText.Substring(i + 1);
            }
            
            infoText.text = portuguesePart + japanesePart;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        infoPanel.SetActive(false);
    }
} 