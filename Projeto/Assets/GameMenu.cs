using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private string nomeCenaDoJogo = "GameScene";
    [SerializeField] private GameObject painelConfiguracoes;

    // Chamado quando o botão "Play" for clicado
    public void Jogar()
    {
        SceneManager.LoadScene(nomeCenaDoJogo);
    }

    // Chamado quando o botão "Sair" for clicado
    public void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();

        // Isso só aparece no Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Chamado quando o botão "Configurações" for clicado
    public void AbrirConfiguracoes()
    {
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(true);
    }

    // Pode ser usado por um botão "Voltar" nas configurações
    public void FecharConfiguracoes()
    {
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);
    }
}
