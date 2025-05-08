using UnityEngine;

public class VisualChanger : MonoBehaviour
{
    [Header("Backgrounds")]
    public Sprite[] backgrounds;
    public SpriteRenderer backgroundRenderer;

    [Header("Neve (animações)")]
    public GameObject nevePai; // GameObject que tem os filhos com animações associadas aos backgrounds

    private GameObject[] neveAnimacoes;
    private int currentIndex = 0;

    void Start()
    {
        // Validação
        if (backgroundRenderer == null || backgrounds.Length == 0 || nevePai == null)
        {
            Debug.LogError("Verifique se todas as referências foram atribuídas!");
            enabled = false;
            return;
        }

        // Coleta os filhos do objeto nevePai
        int filhoCount = nevePai.transform.childCount;
        neveAnimacoes = new GameObject[filhoCount];

        for (int i = 0; i < filhoCount; i++)
        {
            neveAnimacoes[i] = nevePai.transform.GetChild(i).gameObject;
        }

        // Validação de tamanho correspondente
        if (backgrounds.Length != neveAnimacoes.Length)
        {
            Debug.LogWarning("O número de backgrounds é diferente do número de animações de neve!");
        }

        AtualizarVisual();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex + 1) % Mathf.Min(backgrounds.Length, neveAnimacoes.Length);
            AtualizarVisual();
        }
    }

    void AtualizarVisual()
    {
        // Atualiza fundo
        if (currentIndex < backgrounds.Length)
            backgroundRenderer.sprite = backgrounds[currentIndex];

        // Ativa só a animação associada ao índice atual
        for (int i = 0; i < neveAnimacoes.Length; i++)
        {
            neveAnimacoes[i].SetActive(i == currentIndex);
        }

        Debug.Log($"➡️ Mudou para índice {currentIndex}: fundo '{backgroundRenderer.sprite?.name}', neve '{neveAnimacoes[currentIndex].name}'");
    }
}
