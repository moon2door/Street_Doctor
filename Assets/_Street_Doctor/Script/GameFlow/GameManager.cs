using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GaugeManager gaugeManager;
    public TimerManager timerManager;
    public Text resultText;
    public Text cprCountText;

    public GameObject ambulance;
    public GameObject phoneObj;
    public Transform pointB;
    public Transform pointC;

    public Image fadeImage;
    public Image newFadeImage;

    private bool isGameOver = false;
    private bool phone_TF = false;

    public bool isGameStart = false;
    public bool CanNextScene = true;
    public bool gameClear = false;

    public int currentCPRCount = 0;
    public int currentBreathCount = 0;
    public bool isCPRPhase = true; // true: CPR, false: breath

    private string triggeredObjectName = null;

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
        
    }

    void Update()
    {
        if (isGameOver || !isGameStart) return;

        if (gaugeManager.IsGaugeEmpty())
        {
            GameFail();
            CanNextScene = true;
        }
        else if (timerManager.IsTimeUp())
        {
            StartCoroutine(HandleSuccessSequence());
        }
    }

    public void StartGamePhase()
    {
        isGameStart = true;
        gaugeManager.ResetGauge();
        timerManager.ResetTimer();
        currentCPRCount = 0;
        currentBreathCount = 0;
        isCPRPhase = true;
        UpdateCPRUI();
    }

    public void TriggerCPR()
    {
        if (!isGameStart || !isCPRPhase) return;

        currentCPRCount++;
        UpdateCPRUI();

        if (currentCPRCount >= 30)
        {
            isCPRPhase = false;
            currentBreathCount = 0;
            UpdateCPRUI();
        }

        gaugeManager.GaugeTrigger();
    }

    public void TriggerBreath()
    {
        if (!isGameStart || isCPRPhase) return;

        currentBreathCount++;
        UpdateCPRUI();

        if (currentBreathCount >= 2)
        {
            isCPRPhase = true;
            currentCPRCount = 0;
            UpdateCPRUI();
        }

        gaugeManager.MouseTrigger();
    }

    void UpdateCPRUI()
    {
        if (cprCountText == null) return;

        if (isCPRPhase)
            cprCountText.text = $"심폐소생술을 진행해야 합니다!\n양 손을 모아 가슴의 빨간 큐브를 \n충분히 눌렀다 떼세요.\n\n흉부압박 진행중 {currentCPRCount}회 / 30회";
        else
            cprCountText.text = $"이제 인공호흡을 해야해요!\n입의 빨간 큐브에 얼굴을 \nn초이상 가져다 대세요.\n\n인공호흡 진행중 {currentBreathCount}회 / 2회";
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

        CanNextScene = true;

        StartCoroutine(WaitAndChangeScene());


    }

    public IEnumerator FadeToBlack()
    {
        float fadeDuration = 2f;
        float elapsed = 0f;
        Image targetImage = SceneManager.GetActiveScene().name == "3_Cut" ? newFadeImage : fadeImage;

        if (targetImage != null)
        {
            targetImage.color = new Color(0f, 0f, 0f, 0f);
            targetImage.gameObject.SetActive(true);

            while (elapsed < fadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                Color c = targetImage.color;
                c.a = alpha;
                targetImage.color = c;

                elapsed += Time.deltaTime;
                yield return null;
            }

            Color finalColor = targetImage.color;
            finalColor.a = 1f;
            targetImage.color = finalColor;
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
        if (scene.name == "4_CPR")
        {
            AssignUIObjects();
            isGameStart = true;
            phoneObj.SetActive(true);
            phone_TF = true;
        }
        else if (scene.name == "3_Cut")
        {
            AssignUIObjects_01();

            phone_TF = false;

            phoneObj.SetActive(false);
        }
        else
        {
            phone_TF = false;

            phoneObj.SetActive(false);
        }
    }

    void AssignUIObjects()
    {
        resultText = GameObject.Find("resultText")?.GetComponent<Text>();
        cprCountText = GameObject.Find("cprCountText")?.GetComponent<Text>();

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

    void AssignUIObjects_01()
    {
        newFadeImage = GameObject.Find("NewFadeOut")?.GetComponent<Image>();
    }

    public void SetTriggeredObject(string objName)
    {
        triggeredObjectName = objName;
        Debug.Log("[Trigger] 플레이어가 닿은 오브젝트: " + objName);
        StartCoroutine(WaitAndChangeScene());
    }


    IEnumerator WaitAndChangeScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "4_CPR")
        {
            if (gameClear)
            {
                DontDestroy.nextSceneName = "5_CPREnding";
            }
            else if (!gameClear)
            {
                gameClear = true;
                DontDestroy.nextSceneName = "1_Start";
            }
        }
        else if (currentScene == "5_CPREnding")
        {
            if (triggeredObjectName == "StartTrigger") // 처음으로 트리거
            {
                DontDestroy.nextSceneName = "1_Start";
            }
            else
            {
                Debug.LogWarning("트리거된 오브젝트가 없습니다.");
                yield break;
            }
        }
        else
        {
            Debug.LogWarning("예상치 못한 씬 이름: " + currentScene);
            yield break;
        }

        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene("2_Load");
    }

}