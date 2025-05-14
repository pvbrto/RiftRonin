using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private GameObject btnMenu;
    [SerializeField] private GameObject btnExit;
    public float duration = 3f;
    public float height;

    private RectTransform rectTransform;
    private float elapsedTime = 0f;

    void Start()
    {
        btnMenu.SetActive(false);
        btnExit.SetActive(false);
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(MoveUp());
    }

    System.Collections.IEnumerator MoveUp()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, height);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;

        yield return new WaitForSeconds(1);
        btnMenu.SetActive(true);
        btnExit.SetActive(true);
    }

    public void Menu(){
        SceneManager.LoadScene("Menu");
    }
    
    public void Exit(){
        SceneManager.LoadScene("Menu");
    }
}
