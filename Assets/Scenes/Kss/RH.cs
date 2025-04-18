using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RH : MonoBehaviour
{
    GameObject right_H;                  // 오른손 앵커
    public GaugeManager gaugeManager;    // 게이지 관리자
    public GameManager gameManager;      // 게임 상태 관리자
    public Tothelastscene tothelastscene;

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
        // 오른손 위치에 오브젝트 붙이기
        right_H = GameObject.Find("RightHandAnchor");
        transform.position = right_H.transform.position;
        transform.parent = right_H.transform;
    }

    void Update()
    {
        // 오른손 트리거 입력 시 게이지 채우기
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            // 게임이 끝나지 않았을 때만 게이지가 참.
            if (gameManager != null && !gameManager.IsGameOver())
            {
                if (SceneManager.GetActiveScene().name == "_InGame_Scene_01")
                {
                    // 게이지 매니저 의 게이지 트리거 메서드 실행
                    gaugeManager.GaugeTrigger();
                }

                else if (SceneManager.GetActiveScene().name == "_Webtoon_Scene_01")
                {
                    tothelastscene.ShowNextImage();
                }
            }
            else
            {
                // 아니면 실행안함
                return;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            if (SceneManager.GetActiveScene().name == "_Webtoon_Scene_01")
            {
                if (tothelastscene.allImagesShown)
                {
                    StartCoroutine(ChangeScene());
                }
            }
            else if (gameManager.CanNextScene)
            {
                StartCoroutine(ChangeScene());
                gameManager.CanNextScene = false;
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // C 씬이 병합되었을 때만 실행
        if (scene.name == "_InGame_Scene_01")
        {
            AssignUIObjects();
        }
    }

    void AssignUIObjects()
    {
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
    }

    IEnumerator ChangeScene()
    {
        // 현재 씬 이름 확인
        string currentScene = SceneManager.GetActiveScene().name;

        // 다음 병합할 씬 설정
        if (currentScene == "____Start")
            DontDestroy.nextSceneName = "_Webtoon_Scene_01";
        else if (currentScene == "_Webtoon_Scene_01")
            DontDestroy.nextSceneName = "_InGame_Scene_01";

        yield return StartCoroutine(gameManager.FadeToBlack());

        SceneManager.LoadScene("__Loading_Scene"); // B 씬 (로딩 씬)으로 이동
    }
}
