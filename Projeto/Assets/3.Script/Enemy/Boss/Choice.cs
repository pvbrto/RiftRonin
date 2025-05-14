using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Choice : MonoBehaviour
{
    private bool playerCanMoveBefore;
    private bool playerInvincibleBefore;
    private bool playerCanReceiveInputBefore;
    private ChangeCursor cursorController;
    [SerializeField] private GameObject square;
    [SerializeField] private GameObject btnJuntar;
    [SerializeField] private GameObject btnLutar;
    [SerializeField] private GameObject text;
    [SerializeField] private GameObject animObj;
    [SerializeField] private Animator anim;

    private void Start()
    {
        cursorController = FindAnyObjectByType<ChangeCursor>();
        DisableGameplay();
    }

    private void DisableGameplay()
    {
        cursorController.gameObject.SetActive(false);

        // Desabilitar controles do jogador
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            // Armazenar estado atual para restaurar depois
            playerCanMoveBefore = player.canMove;
            playerCanReceiveInputBefore = player.canReceiveInput; // Nova variável
            playerInvincibleBefore = player.invincible;

            // Desativar TODAS as entradas e tornar invencível durante o diálogo
            player.canMove = false;
            player.canReceiveInput = false; // Desativar todas as entradas
            player.invincible = true;

        }
    }

    private void EnableGameplay()
    {
        ChangeCursor cursorController = FindAnyObjectByType<ChangeCursor>();
        cursorController.gameObject.SetActive(true);

        square.SetActive(false);

        btnJuntar.SetActive(false);
        btnLutar.SetActive(false);
        text.SetActive(false);

        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.canMove = playerCanMoveBefore;
            player.canReceiveInput = playerCanReceiveInputBefore;
            player.invincible = playerInvincibleBefore;
        }
    }

    public void lutar()
    {
        EnableGameplay();
    }

    public void juntar()
    {
        // EnableGameplay();
        StartCoroutine(StartTransition());
    }

    IEnumerator StartTransition(){
        animObj.SetActive(true);
        // anim.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("TelaFimLutar");
    }
}