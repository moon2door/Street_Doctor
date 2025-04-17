using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GaugeManager gaugeManager;       // 게이지 관리 스크립트
    public TimerManager timerManager;       // 타이머 관리 스크립트
    public Text resultText;                 // 결과 텍스트 (성공/실패 출력용)

    public GameObject ambulance;            // 구급차
    public Transform pointB;                // 시작점
    public Transform pointC;                // 도착점

    public Image fadeImage;                 // 화면을 어둡게 덮을 단일 UI 이미지 (검정색)

    private bool isGameOver = false;        // 게임 종료 여부 플래그
    private bool isGameStart = false;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        RestartGame(); // 게임 시작 시 초기화
    }

    void Update()
    {
        if (isGameOver || !isGameStart) return;

        if (gaugeManager.IsGaugeEmpty())
        {
            GameFail();
        }
        else if (timerManager.IsTimeUp())
        {
            StartCoroutine(HandleSuccessSequence());
        }
    }

    void GameFail()
    {
        isGameOver = true;
        resultText.text = "실패!";
        gaugeManager.StopGauge();
        timerManager.StopTimer();
    }

    IEnumerator HandleSuccessSequence()
    {
        isGameOver = true;

        gaugeManager.StopGauge();
        timerManager.StopTimer();

        ambulance.SetActive(true);
        ambulance.transform.position = pointB.position;

        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ambulance.transform.position = Vector3.Lerp(pointB.position, pointC.position, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ambulance.transform.position = pointC.position;

        yield return StartCoroutine(FadeToBlack());

        ambulance.SetActive(false);

        resultText.text = "성공!";
    }

    public IEnumerator FadeToBlack()
    {
        float fadeDuration = 2f;
        float elapsed = 0f;

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0f, 0f, 0f, 0f);
            fadeImage.gameObject.SetActive(true);

            while (elapsed < fadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;

                elapsed += Time.deltaTime;
                yield return null;
            }

            Color finalColor = fadeImage.color;
            finalColor.a = 1f;
            fadeImage.color = finalColor;
        }
    }

    public IEnumerator FadeReturn()
    {
        fadeImage.color = new Color(0, 0, 0, 0);

        yield return null;
    }

    public void RestartGame()
    {
        isGameOver = false;
        resultText.text = "";

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0f, 0f, 0f, 0f);
            fadeImage.gameObject.SetActive(true);
        }

        ambulance.SetActive(false);
        gaugeManager.ResetGauge();
        timerManager.ResetTimer();
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "C_Scene")
        {
            AssignUIObjects();
            isGameStart = true;
        }
    }

    void AssignUIObjects()
    {
        resultText = GameObject.Find("resultText")?.GetComponent<Text>();
        ambulance = GameObject.Find("ambulance");
        pointB = GameObject.Find("pointB")?.transform;
        pointC = GameObject.Find("pointC")?.transform;
        fadeImage = GameObject.Find("FadeOut_Image")?.GetComponent<Image>();

        var gaugeManagerObj = GameObject.Find("GaugeManager");
        if (gaugeManagerObj != null)
        {
            gaugeManager = gaugeManagerObj.GetComponent<GaugeManager>();
            if (gaugeManager.gaugeSlider == null)
            {
                var sliderObj = GameObject.Find("gaugeSlider");
                if (sliderObj != null)
                    gaugeManager.gaugeSlider = sliderObj.GetComponent<Slider>();
            }
        }

        var timerManagerObj = GameObject.Find("TimerManager");
        if (timerManagerObj != null)
        {
            timerManager = timerManagerObj.GetComponent<TimerManager>();
            if (timerManager.timerText == null)
            {
                var textObj = GameObject.Find("timerText");
                if (textObj != null)
                    timerManager.timerText = textObj.GetComponent<Text>();
            }
        }

        ambulance.SetActive(false);
    }
}
