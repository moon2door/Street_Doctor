using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipManager : MonoBehaviour
{
    [Header("튜토리얼 UI들 (순서대로)")]
    public GameObject[] tutorialCanvases;// 순서대로 보여줄 튜토리얼 UI 오브젝트들

    private int currentIndex = 0;          // 현재 보여지고 있는 튜토리얼 인덱스
    private bool isAHeld = false;          // A 버튼을 누르고 있는지 여부
    private float holdTime = 0f;           // A 버튼 누르고 있는 시간
    public float holdThreshold = 2f;     // 2초 이상이면 전체 스킵

    private GameManager gameManager; // 내부에서 런타임에 찾을 GameManager

    void Start()
    {
        ShowCanvas(0); // 시작 시 첫 UI만 보여줌

        // ✅ 런타임에 GameManager 자동 찾기
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("[SkipManager] GameManager를 찾을 수 없습니다.");
        }
    }
    void Update()
    {
        // A 버튼을 누르기 시작한 순간
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            isAHeld = true;
            holdTime = 0f;
        }
        // A 버튼을 계속 누르고 있는 동안 시간 누적
        if (isAHeld)
        {
            holdTime += Time.deltaTime;
        }
        // A 버튼에서 손을 뗀 순간
        if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            isAHeld = false;

            // 2초 이상 누른 경우: 전체 튜토리얼 스킵
            if (holdTime >= holdThreshold)
            {
                SkipAllTutorials();
            }
            else // 2초 미만: 다음 UI로 넘어감 (작은 스킵)
            {
                SkipOneTutorial();
            }
        }
    }


    // 전달된 인덱스의 튜토리얼만 활성화, 나머지는 모두 비활성화
    void ShowCanvas(int index)
    {
        for (int i = 0; i < tutorialCanvases.Length; i++)
        {
            tutorialCanvases[i].SetActive(i == index);
        }
    }
    // 현재 튜토리얼 UI를 끄고 다음 UI를 켬
    void SkipOneTutorial()
    {
        if (currentIndex < tutorialCanvases.Length)
        {
            tutorialCanvases[currentIndex].SetActive(false);
            currentIndex++;
        }
        if (currentIndex < tutorialCanvases.Length)
        {
            tutorialCanvases[currentIndex].SetActive(true);
        }
        else
        {
            Debug.Log("튜토리얼 끝");

            if (SceneManager.GetActiveScene().name == "4_CPR")
            {
                // 체력바 활성화
                // cpr 횟수 활성화
                // 튜토리얼 off 
                // 게임스타트 실행
            }
        }
    }
    // 모든 튜토리얼 UI를 한 번에 꺼버림 (전체 스킵)
    void SkipAllTutorials()
    {
        Debug.Log("[SkipManager] SkipAllTutorials() 호출됨");
        foreach (var canvas in tutorialCanvases)
        {
            if (canvas != null)
                canvas.SetActive(false);
        }
        Debug.Log($"[SkipManager] 현재 씬 이름: {SceneManager.GetActiveScene().name}");

        //  CPR 튜토리얼인 경우 GameManager에 반영
        if (SceneManager.GetActiveScene().name == "4_CPR" && gameManager != null)
        {
            Debug.Log("[SkipManager] 4_CPR에서 tutorial 모드 강제 종료 시도");

            if (gameManager.tutorial)
            {
                gameManager.tutorial = false;              //  튜토리얼 비활성화
                gameManager.currentCPRCount = 0;           //  카운트 리셋
                gameManager.currentBreathCount = 0;
                gameManager.StartGamePhase();              //  일반 CPR 루틴 시작
                Debug.Log("[SkipManager] CPR 튜토리얼 강제 종료");
            }
        }
        else
        {
            Debug.LogWarning("[SkipManager] 조건 불충족: 4_CPR + tutorial == true가 아님");
        }
    }
}