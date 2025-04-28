using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GaugeManager gaugeManager;
    public TimerManager timerManager;
    public JawTiltController jawTiltController;


    public Text cprCountText;

    public GameObject ambulance;
    public GameObject phoneObj;
    public GameObject fingerRhand;

    public Transform start_Point;
    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    public Transform pointD;
    public Transform end_Point;

    public Image fadeImage;
    public Image newFadeImage;

    private bool isGameOver = false;
    private bool phone_TF = false;

    public bool isGameStart = false;
    public bool tutorial = true;
    public bool CanNextScene = true;
    public bool gameClear = true;

    public int currentCPRCount = 0;
    public int currentBreathCount = 0;
    public int currentJawCount = 0;
    public bool isCPRPhase = true; // true: CPR, false: breath

    private string triggeredObjectName = null;

    public GameObject fail_Image;
    public GameObject cprCanvus;
    public GameObject gaugeCanvus;

    public AudioClip effectSound;           // 한 번 재생할 사운드 클립
    public AudioClip cprSound;           // 한 번 재생할 사운드 클립
    private AudioSource audioSource;        // AudioSource 컴포넌트

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
        audioSource = GetComponent<AudioSource>();
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
        currentCPRCount = 0;
        currentBreathCount = 0;
        isCPRPhase = true;
        gaugeManager.ResetGauge();
        timerManager.ResetTimer();
        UpdateCPRUI();
    }

    public void TriggerCPR()
    {
        if (!isGameStart || !isCPRPhase) return;

        audioSource.PlayOneShot(cprSound);
        currentCPRCount++;
        UpdateCPRUI();

        if (currentCPRCount >= 60)
        {
            audioSource.PlayOneShot(effectSound);

            isCPRPhase = false;
            currentBreathCount = 0;
            UpdateCPRUI();
        }

        gaugeManager.GaugeTrigger();
    }

    public void TriggerBreath()
    {
        if (!isGameStart || isCPRPhase || !jawTiltController.hasActivated) return;

        audioSource.PlayOneShot(effectSound);

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

    public void Triggerjaw()
    {
        if (!isGameStart || isCPRPhase) return;
        
        currentJawCount++;
        UpdateCPRUI();
        audioSource.PlayOneShot(effectSound);
    }

    void UpdateCPRUI()
    {
        if (cprCountText == null) return;

        if (tutorial)
        {
            if (isCPRPhase)
                cprCountText.text =
                    $"흉부압박 진행중 {currentCPRCount}회 / 60회";

            else if (!isCPRPhase && jawTiltController.hasActivated)
                cprCountText.text =
                    $"인공호흡 진행중 {currentBreathCount}회 / 2회";
            else
                cprCountText.text =
                    $"환자의 고개 젖히기 (최고 1회만 실행) 0 / 1회";
        }
        else if (!tutorial)
        {
            if (isCPRPhase)
                cprCountText.text =
                    $"흉부압박 진행중 {currentCPRCount}회 / 60회";
            else if (!isCPRPhase && jawTiltController.hasActivated)
                cprCountText.text =
                    $"인공호흡 진행중 {currentBreathCount}회 / 2회";

            else
                cprCountText.text =
                    $"환자의 고개 젖히기 (최고 1회만 실행) 0 / 1회";
        }


    }

    void GameFail()
    {
        isGameOver = true;
        gameClear = false;
        fail_Image.SetActive(true);
        gaugeManager.StopGauge();
        timerManager.StopTimer();
        StartCoroutine(WaitAndChangeScene());
    }

    IEnumerator HandleSuccessSequence()
    {
        isGameOver = true;
        gameClear = true;

        gaugeManager.StopGauge();
        timerManager.StopTimer();

        ambulance.SetActive(true);
        ambulance.transform.position = start_Point.position;

        // 포인트 배열과 회전값 설정
        Transform[] points = new Transform[] { pointA, pointB, pointC, pointD, end_Point };
        float[] rotations = new float[] { 90f, 180f, 270f, 180f };

        // 총 이동에 걸리는 시간
        float totalDuration = 5f; // (원하는 값으로 설정)

        // 거리 계산
        float[] distances = new float[points.Length];
        float totalDistance = 0f;

        Vector3 previousPos =  start_Point.position;

        for (int i = 0; i < points.Length; i++)
        {
            distances[i] = Vector3.Distance(previousPos, points[i].position);
            totalDistance += distances[i];
            previousPos = points[i].position;
        }

        // 이동 시작
        previousPos = start_Point.position;
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 startPos = previousPos;
            Vector3 endPos = points[i].position;

            float moveDuration = totalDuration * (distances[i] / totalDistance); // 거리비율만큼 시간 할당
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                ambulance.transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            ambulance.transform.position = endPos;

            // 회전값 변경 (B~E 지점 도착 시)
            if (i < rotations.Length)
            {
                Vector3 currentEulerAngles = ambulance.transform.eulerAngles;
                currentEulerAngles.y = rotations[i];
                ambulance.transform.eulerAngles = currentEulerAngles;
            }

            previousPos = endPos;
        }


        CanNextScene = true;

        StartCoroutine(WaitAndChangeScene());


    }

    public IEnumerator FadeToBlack()
    {
        if (SceneManager.GetActiveScene().name == "4_CPR")
        {
            if (!gameClear)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        if (SceneManager.GetActiveScene().name == "5_CPREnding") yield break;

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
        fail_Image.SetActive(false);

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
            fingerRhand.SetActive(true);
            phone_TF = true;
        }
        else if (scene.name == "3_Cut")
        {
            AssignUIObjects_01();

            phone_TF = false;

            phoneObj.SetActive(false);
            fingerRhand.SetActive(false);
        }
        else
        {
            phone_TF = false;

            phoneObj.SetActive(false);
            fingerRhand.SetActive(false);
        }
    }

    void AssignUIObjects()
    {
        cprCountText = GameObject.Find("cprCountText")?.GetComponent<Text>();

        jawTiltController = GameObject.Find("Jaw_Control")?.GetComponent<JawTiltController>();

        ambulance = GameObject.Find("ambulance");

        start_Point = GameObject.Find("Start Point")?.transform;
        end_Point = GameObject.Find("End Point")?.transform;
        pointA = GameObject.Find("Point A")?.transform;
        pointB = GameObject.Find("Point B")?.transform;
        pointC = GameObject.Find("Point C")?.transform;
        pointD = GameObject.Find("Point D")?.transform;

        fadeImage = GameObject.Find("FadeOut_Image")?.GetComponent<Image>();
        fail_Image = GameObject.Find("Failed__00000");
        cprCanvus = GameObject.Find("CPR Canvus");
        gaugeCanvus = GameObject.Find("Gauge Canvas");

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
        fail_Image.SetActive(false);
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
                Debug.LogError("5씬 열림");
            }
            else if (!gameClear)
            {
                gameClear = true;
                DontDestroy.nextSceneName = "1_Start";
                gaugeCanvus.SetActive(false);
                cprCanvus.SetActive(false);
                fail_Image.SetActive(true);
                Debug.LogError("1씬 열림");
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