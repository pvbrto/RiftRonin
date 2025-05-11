using UnityEngine;
using TMPro;

public class InteractionIndicator : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI indicatorText;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float hoverDistance = 2f;

    private CanvasGroup canvasGroup;
    private bool isHovering = false;
    private Camera mainCamera;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        mainCamera = Camera.main;
        
        // Inicia com o indicador invisível
        canvasGroup.alpha = 0f;
        indicatorText.text = "E";
    }

    private void Update()
    {
        // Verifica se o mouse está sobre o player
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hoverDistance))
        {
            if (hit.collider.CompareTag("Player") && !isHovering)
            {
                isHovering = true;
                ShowIndicator();
            }
            else if (!hit.collider.CompareTag("Player") && isHovering)
            {
                isHovering = false;
                HideIndicator();
            }
        }
        else if (isHovering)
        {
            isHovering = false;
            HideIndicator();
        }
    }

    private void ShowIndicator()
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    private void HideIndicator()
    {
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
    }
} 