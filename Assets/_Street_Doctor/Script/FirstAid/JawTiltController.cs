using UnityEngine;
using UnityEngine.SceneManagement;

public class JawTiltController : MonoBehaviour
{
    // 고개 젖히기에 사용할 Transform들 (캐릭터 머리와 손 위치)
    public Transform head;          // 머리 본
    public Transform headEnd;       // 손이 닿는 기준 위치
    public Transform rightHand;     // 오른손 위치
    public Transform leftHand;      // 왼손 위치

    // 고개 젖히기 트리거 조건 및 동작 파라미터들
    public float activationDistance = 0.1f;   // 손이 일정 거리 이내에 들어오면 작동
    public float tiltAngle = -15f;            // 머리를 뒤로 젖히는 각도
    public float rotationSpeed = 2f;          // 고개 젖히는 속도 (Lerp 보간 속도)
    public float moveAmountZ = 0.03f;         // 머리를 뒤로 이동시키는 거리(Z축 기준)

    private bool isTilting = false;           // 현재 고개를 젖히는 중인지 여부
    private Quaternion targetRotation;        // 목표 회전값
    private Vector3 targetPosition;           // 목표 위치값

    public bool hasActivated = false;         // 고개 젖히기 완료 여부 (중복 방지)

    public GameManager gameManager;           // 게임 매니저 참조 (Jaw 완료 후 진행을 위한 콜백용)

    private Quaternion initialRotation; // 초기 회전값 저장용

    // 씬이 로드될 때 콜백 등록
    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        initialRotation = head.localRotation; // 시작 시 회전값 저장
    }

    void Update()
    {
        // 이미 고개 젖힘 완료했거나, 필수 오브젝트 없으면 리턴
        if (hasActivated || head == null || headEnd == null || (rightHand == null && leftHand == null) || gameManager.isCPRPhase)
            return;

        // 손 위치와 headEnd 사이 거리 계산
        float rightDist = rightHand != null ? Vector3.Distance(rightHand.position, headEnd.position) : float.MaxValue;
        float leftDist = leftHand != null ? Vector3.Distance(leftHand.position, headEnd.position) : float.MaxValue;

        // 어느 한 손이라도 일정 거리 이내에 들어오면 고개 젖히기 시작
        if (rightDist < activationDistance || leftDist < activationDistance)
        {
            Debug.Log("🟢 기도 확보 조건 충족 → 고개 천천히 젖히기 시작");

            // 회전 시작 시 목표 회전값을 "기준 회전 + tiltAngle"로 고정
            targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x + tiltAngle, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);
            //targetPosition = head.localPosition + new Vector3(0f, 0f, moveAmountZ);
            isTilting = true;
        }

        // 고개 젖히는 중이면 보간 방식으로 천천히 이동
        if (isTilting)
        {
            head.localRotation = Quaternion.Lerp(head.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
            //head.localPosition = Vector3.Lerp(head.localPosition, targetPosition, Time.deltaTime * rotationSpeed);

            // 목표 위치와 회전에 거의 도달하면 완료 처리
            if (Quaternion.Angle(head.localRotation, targetRotation) < 0.5f)
            {
                isTilting = false;
                hasActivated = true;
                gameManager.Triggerjaw();
            }
        }
    }

    // 씬이 로드될 때 자동 호출
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 특정 씬("4_CPR")에서만 UI 오브젝트 재연결
        if (scene.name == "4_CPR") AssignUIObjects();
    }

    // 오브젝트 자동 참조 연결
    void AssignUIObjects()
    {
        // 오른손/왼손 위치 찾아서 할당
        rightHand = GameObject.Find("Right_Hand")?.GetComponent<Transform>();
        leftHand = GameObject.Find("Left_Hand")?.GetComponent<Transform>();

        // 게임 매니저 참조 연결
        gameManager = GameObject.Find("GameManager")?.GetComponent<GameManager>();
    }
}
