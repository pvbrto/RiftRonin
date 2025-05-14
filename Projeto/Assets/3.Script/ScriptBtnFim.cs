using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptBtnFim : MonoBehaviour
{
    [SerializeField] private GameObject btnMenu;
    [SerializeField] private GameObject btnExit;

    public void Menu(){
        SceneManager.LoadScene("Menu");
    }
    
    public void Exit(){
        Application.Quit();
    }
}
