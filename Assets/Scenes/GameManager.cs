using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GaugeManager gaugeManager;       // 게이지 관리 스크립트
    public TimerManager timerManager;       // 타이머 관리 스크립트
    public Text resultText;                 // 결과 텍스트 (성공/실패 출력용)

    public GameObject ambulance; // 구급차
    public Transform pointB; // 시작점
    public Transform pointC; // 도착점

    public Image[] fadeImages; // 화면을 어둡게 덮을 UI 이미지 (검정색)

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
        if (isGameOver) return; // 게임 종료 시 아무것도 하지 않음
        if (!isGameStart) return; 

        // 게이지가 다 닳았을 경우 실패 처리
        if (gaugeManager.IsGaugeEmpty())
        {
            GameFail();
        }
        // 타이머가 끝났을 경우 성공 처리
        else if (timerManager.IsTimeUp())
        {
            StartCoroutine(HandleSuccessSequence());
        }
    }

    void GameFail()
    {
        // 게임실패 메서드가 실행되면
        // 게임오버 = true
        isGameOver = true;
        // 텍스트 실패 띄우기
        resultText.text = "실패!";
        // 게이지매니저의 스탑게이지 실행
        gaugeManager.StopGauge();
    }

    IEnumerator HandleSuccessSequence()
    {
        // 게임오버 (게임 중지) true
        isGameOver = true;

        // 게이지 매니저의 스탑게이지 실행
        gaugeManager.StopGauge();

        // 엠뷸 위치 초기화 및 활성화
        ambulance.SetActive(true);
        ambulance.transform.position = pointB.position;

        float duration = 3f;
        float elapsed = 0f;

        // 엠뷸 자연스럽게 이동
        while (elapsed < duration)
        {
            ambulance.transform.position = Vector3.Lerp(pointB.position, pointC.position, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 엠뷸 위치 포인트c로 값 재 확인
        ambulance.transform.position = pointC.position;

        // 화면 천천히 까매지기
        yield return StartCoroutine(FadeToBlack());

        // 성공 텍스트 출력
        resultText.text = "성공!";
    }

    IEnumerator FadeToBlack() // 페이드인 만들었는데 이거 쓰지말구 주영이 형이 만든 페이드인 쓰세요
    {
        float fadeDuration = 2f;
        float elapsed = 0f;

        // 초기 알파값 0으로 설정
        foreach (var img in fadeImages)
        {
            img.color = new Color(0f, 0f, 0f, 0f);
            img.gameObject.SetActive(true); // 혹시 꺼져 있다면 활성화
        }

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            foreach (var img in fadeImages)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 최종 알파값 1로 고정
        foreach (var img in fadeImages)
        {
            Color c = img.color;
            c.a = 1f;
            img.color = c;
        }
    }

    public void RestartGame() // 게임 재시작메서드
    {
        isGameOver = false; // 게임오버 값 false 
        resultText.text = ""; // 결과값 텍스트 비우기 (성공 & 실패)

        foreach (var img in fadeImages)
        {
            img.color = new Color(0f, 0f, 0f, 0f); // 알파 0으로 초기화
            img.gameObject.SetActive(true); // 필요 시 켜두기
        }
        ambulance.SetActive(false); // 구급차 숨기기

        // 게이지 및 타이머 초기화
        gaugeManager.ResetGauge();
        timerManager.ResetTimer();
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // C 씬이 병합되었을 때만 실행
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

        var gaugeManagerObj = GameObject.Find("GaugeManager");
        if (gaugeManagerObj != null)
        {
            gaugeManager = gaugeManagerObj.GetComponent<GaugeManager>();
            // 내부 gaugeSlider 할당도 수행
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
            // 내부 timerText 할당도 수행
            if (timerManager.timerText == null)
            {
                var textObj = GameObject.Find("timerText");
                if (textObj != null)
                    timerManager.timerText = textObj.GetComponent<Text>();
            }
        }
    }
}
