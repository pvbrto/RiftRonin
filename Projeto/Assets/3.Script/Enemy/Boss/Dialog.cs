using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialog : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private float typingSpeed = 0.05f; // Velocidade entre cada caractere
    [SerializeField] private float delayBeforeStart = 1f; // Delay antes de começar a digitação
    [SerializeField] private float delayBeforeTranslation = 2f; // Delay antes de começar a tradução
    [SerializeField] private string japaneseText = "こんにちは"; // Texto em japonês
    [SerializeField] private GameObject btnJuntar;
    [SerializeField] private GameObject btnLutar;

    private string fullText;
    private string currentText = "";

    private void Start()
    {
        // Guarda o texto completo e mostra o texto em japonês
        fullText = titleText.text;
        titleText.text = japaneseText;
        
        // Inicia a animação
        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        // Espera o delay inicial
        yield return new WaitForSeconds(delayBeforeStart);

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
            
            currentText = portuguesePart + japanesePart;
            titleText.text = currentText;
            yield return new WaitForSeconds(typingSpeed);
        }

        btnJuntar.SetActive(true);
        btnLutar.SetActive(true);
    }

    public void lutar(){

    }

    public void juntar(){
        
    }
} 