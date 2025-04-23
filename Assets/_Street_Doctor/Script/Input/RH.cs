using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RH : MonoBehaviour
{
    LineRenderer myLR;                  // 상호작용 라인을 그릴 라인렌더러
    GameObject right_H;                // 오른손 앵커 객체
    Ray ray;                            // 상호작용용 Ray
    RaycastHit hit;                     // Ray 충돌 정보

    public Transform playerRoot;        // 플레이어 루트 오브젝트 (회전용)
    public float rotationSpeed = 45f;   // 플레이어 회전 속도

    public GaugeManager gaugeManager;   // 게이지 매니저
    public GameManager gameManager;     // 게임 매니저
    public Tothelastscene tothelastscene; // 이미지 재생 후 씬 전환 클래스
    public Ending_Manager ending_Manager;

    private GameObject grabbedObject = null; // 현재 잡은 오브젝트
    private bool doorOpened = false;         // 문이 열렸는지 여부

    // 씬 로드 시 이벤트 등록
    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        // 라인렌더러 및 오른손 위치 초기화
        myLR = GetComponent<LineRenderer>();
        right_H = GameObject.Find("RightHandAnchor");
        transform.position = right_H.transform.position;
        transform.eulerAngles = right_H.transform.eulerAngles;
        transform.parent = right_H.transform;

        StartCoroutine(AssignUIObjects001());
    }

    void Update()
    {
        HandleInteractionRay();   // 상호작용용 레이 처리
        HandlePlayerRotation();   // 플레이어 회전 처리
        HandleGrabRelease();      // 잡은 오브젝트 놓기 처리
    }

    // 손에서 나오는 레이로 오브젝트와 상호작용
    void HandleInteractionRay()
    {
        ray.origin = right_H.transform.position;
        ray.direction = right_H.transform.forward;
        myLR.SetPosition(0, ray.origin);
        myLR.SetPosition(1, ray.origin + ray.direction * 3);



        if (SceneManager.GetActiveScene().name == "3_Cut" && !tothelastscene.allImagesShown)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                tothelastscene.ShowNextImage();
            }

        }
        else if (SceneManager.GetActiveScene().name == "3_Cut" && tothelastscene.allImagesShown)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                StartCoroutine(WaitAndChangeScene());
            }
        }

        if (SceneManager.GetActiveScene().name == "5_CPREnding")
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                ending_Manager.Clicker();
            }
        }


        // 충돌한 오브젝트가 있는 경우
        if (Physics.Raycast(ray, out hit, 3f))
        {
            myLR.startColor = Color.green;
            myLR.endColor = Color.green;
            myLR.SetPosition(1, hit.point);

            // 문 상호작용 (문이 열렸는지 체크하고 컷씬으로 전환)
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                DoorInteraction door = hit.collider.gameObject.GetComponentInParent<DoorInteraction>();
                if (door != null)
                {
                    door.OnInteract();
                    doorOpened = true;
                    StartCoroutine(WaitAndChangeScene());

                }
            }

            // 오브젝트 집기
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch) && hit.collider.GetComponent<GrabObject>() != null)
            {
                grabbedObject = hit.collider.gameObject;
                grabbedObject.transform.position = right_H.transform.position + ray.direction * 0.1f;
                grabbedObject.transform.parent = right_H.transform;
                grabbedObject.transform.eulerAngles = Vector3.zero;
            }

            // 오브젝트 고정 지점에 부착
            if (grabbedObject != null && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                if (hit.collider.GetComponent<AttachableSpot>() is AttachableSpot spot && spot.snapTransform != null)
                {
                    grabbedObject.transform.position = spot.snapTransform.position;
                    grabbedObject.transform.rotation = spot.snapTransform.rotation;
                    grabbedObject.transform.parent = spot.snapTransform;
                    StartCoroutine(ShortVibration(0.1f));
                    grabbedObject = null;
                }
            }
        }
        else
        {
            // 레이 충돌 없을 경우 라인 색상 변경
            myLR.startColor = Color.red;
            myLR.endColor = Color.red;
        }
    }

    // 오른쪽 스틱 좌우로 플레이어 회전
    void HandlePlayerRotation()
    {
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (Mathf.Abs(input.x) > 0.5f)
        {
            float rotationAmount = input.x * rotationSpeed * Time.deltaTime;
            playerRoot.Rotate(Vector3.up, rotationAmount);
        }
    }

    // 잡은 오브젝트 놓기 처리
    void HandleGrabRelease()
    {
        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch) && grabbedObject != null)
        {
            grabbedObject.transform.parent = null;
            GameObject location = GameObject.Find(grabbedObject.name + "_");
            if (location != null)
            {
                grabbedObject.transform.eulerAngles = Vector3.zero;
                grabbedObject.transform.parent = location.transform;
                grabbedObject.transform.localPosition = Vector3.zero;
            }
            grabbedObject = null;
        }
    }

    // 페이드 아웃 -> 다음 씬 로드
    IEnumerator WaitAndChangeScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // 현재 씬이 1_Start면 3_Cut으로, 3_Cut이면 4_CPR로 설정
        if (currentScene == "1_Start")
        {
            DontDestroy.nextSceneName = "3_Cut";
        }
        else if (currentScene == "3_Cut")
        {
            DontDestroy.nextSceneName = "4_CPR";
        }
        else
        {
            Debug.LogWarning("예상치 못한 씬 이름: " + currentScene);
            yield break; // 다음 씬을 지정하지 않았으므로 중단
        }

        yield return StartCoroutine(gameManager.FadeToBlack());
        SceneManager.LoadScene("2_Load");
    }

    // 짧은 진동 효과
    IEnumerator ShortVibration(float duration)
    {
        OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }

    // 씬 로드 후 게이지 오브젝트 찾기
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "4_CPR") AssignUIObjects();

        if (scene.name == "5_CPREnding")
        {
             ending_Manager = GameObject.Find("Ending_Manager")?.GetComponent<Ending_Manager>();
        }
    }

    // 씬 내에서 UI 오브젝트 찾기
    void AssignUIObjects()
    {
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
    }

    IEnumerator AssignUIObjects001()
    {
        yield return new WaitForSeconds(0.5f);

        playerRoot = GameObject.Find("PlayerRoot(Clone)")?.GetComponent<Transform>();
    }
}
