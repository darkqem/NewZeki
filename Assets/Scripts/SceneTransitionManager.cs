using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager instance;
    public static SceneTransitionManager Instance => instance;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private Color fadeColor = Color.black;

    [Header("Loading Text Settings")]
    [SerializeField] private bool showLoadingProgress = true;
    [SerializeField] private Color loadingTextColor = Color.white;
    [SerializeField] private int loadingTextFontSize = 36;
    [SerializeField] private TMP_FontAsset customFont;
    [SerializeField] private string loadingTextFormat = "Загрузка: {0}%";
    [SerializeField] private Vector2 loadingTextOffset = Vector2.zero;

    private CanvasGroup fadePanel;
    private TextMeshProUGUI loadingText;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFadePanel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFadePanel()
    {
        // Создаем канвас для затемнения
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.parent = transform;
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999999999; // Поверх всего UI
        
        // Добавляем CanvasScaler для корректного масштабирования
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Создаем панель затемнения
        GameObject panelObj = new GameObject("FadePanel");
        panelObj.transform.parent = canvasObj.transform;
        
        // Настраиваем компоненты панели
        Image fadeImage = panelObj.AddComponent<Image>();
        fadeImage.color = fadeColor;
        fadePanel = panelObj.AddComponent<CanvasGroup>();
        
        // Настраиваем размер и позицию панели
        RectTransform rt = panelObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        // Создаем текст загрузки
        GameObject loadingTextObj = new GameObject("LoadingText");
        loadingTextObj.transform.parent = panelObj.transform;
        loadingText = loadingTextObj.AddComponent<TextMeshProUGUI>();
        loadingText.text = string.Format(loadingTextFormat, 0);
        loadingText.color = loadingTextColor;
        loadingText.fontSize = loadingTextFontSize;
        loadingText.alignment = TextAlignmentOptions.Center;

        // Применяем кастомный шрифт, если он задан
        if (customFont != null)
        {
            loadingText.font = customFont;
        }

        // Настраиваем позицию текста
        RectTransform textRT = loadingText.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0.5f, 0.5f);
        textRT.anchorMax = new Vector2(0.5f, 0.5f);
        textRT.sizeDelta = new Vector2(400, 100);
        textRT.anchoredPosition = loadingTextOffset;
        
        // Изначально панель прозрачная
        fadePanel.alpha = 0;
        fadePanel.blocksRaycasts = false;
        loadingText.alpha = 0;
    }

    public void LoadScene(int sceneIndex)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionCoroutine(sceneIndex));
        }
    }

    private IEnumerator TransitionCoroutine(int sceneIndex)
    {
        isTransitioning = true;

        // Затемнение
        fadePanel.blocksRaycasts = true;
        float elapsedTime = 0;
        while (elapsedTime < transitionDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / (transitionDuration / 2));
            fadePanel.alpha = alpha;
            if (showLoadingProgress)
            {
                loadingText.alpha = alpha;
            }
            yield return null;
        }
        fadePanel.alpha = 1;
        if (showLoadingProgress)
        {
            loadingText.alpha = 1;
        }

        // Загрузка новой сцены
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            if (showLoadingProgress)
            {
                loadingText.text = string.Format(loadingTextFormat, (asyncLoad.progress * 100).ToString("F0"));
            }
            yield return null;
        }

        if (showLoadingProgress)
        {
            loadingText.text = string.Format(loadingTextFormat, "100");
        }

        yield return new WaitForSeconds(0.2f);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Осветление
        elapsedTime = 0;
        while (elapsedTime < transitionDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / (transitionDuration / 2));
            fadePanel.alpha = alpha;
            if (showLoadingProgress)
            {
                loadingText.alpha = alpha;
            }
            yield return null;
        }
        fadePanel.alpha = 0;
        if (showLoadingProgress)
        {
            loadingText.alpha = 0;
        }
        fadePanel.blocksRaycasts = false;

        isTransitioning = false;
    }
} 