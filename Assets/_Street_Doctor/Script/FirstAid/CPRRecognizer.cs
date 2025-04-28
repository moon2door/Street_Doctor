using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CPRRecognizer : MonoBehaviour
{
    public GaugeManager gaugeManager;
    public GameManager gameManager;

    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject headset;

    public float chestY = 0.5f;

    [Tooltip("CPR 눌림 깊이 기준 (단위: 미터, 0.02 = 2cm)")]
    public float pressDepth = 0.02f;  // ✅ 수정됨: 2cm로 설정
    //public float pressDepth = 0.015f;

    private bool leftIn = false;
    private bool rightIn = false;
    public bool isCPRActive { get; private set; } = false;

    private Coroutine vibrationCoroutine = null;

    [Header("Visual Feedback")]
    public Renderer chestRenderer;
    private Material chestMaterial;
    private Color baseColor;
    private bool isGlowing = false;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Start()
    {
        chestY = transform.position.y + 0.03f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Left_Hand") leftIn = true;
        if (other.name == "Right_Hand") rightIn = true;

        if (leftIn && rightIn)
        {
            isCPRActive = true;
            StartGlow();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Left_Hand") leftIn = false;
        if (other.name == "Right_Hand") rightIn = false;

        StopVibration();

        if (!leftIn || !rightIn)
        {
            isCPRActive = false;
            StopGlow();
        }
    }

    private void Update()
    {
        if (leftIn && rightIn)
        {
            float leftY = leftHand.transform.position.y;
            float rightY = rightHand.transform.position.y;
            float avgHandY = (leftY + rightY) / 2f;
            float chestDepth = chestY - avgHandY;

            //bool isPressing = chestDepth > pressDepth - 0.005f;
            bool isPressing = chestDepth > pressDepth;  // ✅ 수정됨: 정확히 2cm 이상 눌러야 함

            //Vector3 chestPos = transform.position;
            //Vector3 handsCenter = (leftHand.transform.position + rightHand.transform.position) * 0.5f;
            //Vector3 handToChestDir = (chestPos - handsCenter).normalized;
            //Vector3 headDir = (chestPos - headset.transform.position).normalized;
            //float alignmentDot = Vector3.Dot(handToChestDir, headDir);
            //bool headIsAbove = alignmentDot > 0.1f;

            //// ✅ [수정됨] 머리가 손보다 일정 높이 이상 위에 있는지 판단
            //float headY = headset.transform.position.y;
            //bool isHeadAbove = headY > avgHandY + 0.01f;

            //// ✅ [수정됨] 머리와 손 사이의 거리 측정 (너무 가까우면 실패)
            //Vector3 handsCenter = (leftHand.transform.position + rightHand.transform.position) * 0.5f;
            //float headToHandDist = Vector3.Distance(headset.transform.position, handsCenter);
            //bool isHeadFarEnough = headToHandDist > 0.4f;

            //// ✅ [수정됨] 최종 CPR 머리 조건: 위에 + 충분히 떨어진 경우
            //bool headIsInCPRPosition = isHeadAbove && isHeadFarEnough;

            // ✅ [수정됨] 머리 위치 판정: 위에 있고, 방향도 위쪽에 가까워야 함
            Vector3 headPos = headset.transform.position;
            Vector3 handsCenter = (leftHand.transform.position + rightHand.transform.position) * 0.5f;
            Vector3 handToHead = headPos - handsCenter;

            bool isAbove = handToHead.y > 0.5f; // ✅ 손보다 50cm 이상 위에
            bool isTooFarBack = Mathf.Abs(handToHead.z) > 0.1f; // ✅ Z축(앞뒤) 방향으로 10cm 초과 벗어나면 안 됨
            bool isTooFarSide = Mathf.Abs(handToHead.x) > 0.1f; // ✅ X축(좌우) 방향으로 10cm 초과 벗어나면 안 됨
            bool isVerticalEnough = handToHead.y > Mathf.Abs(handToHead.x) && handToHead.y > Mathf.Abs(handToHead.z); // ✅ Y축 방향이 제일 우세해야 함

            bool headIsInCPRPosition = isAbove && !isTooFarBack && !isTooFarSide && isVerticalEnough;

            if (isPressing && headIsInCPRPosition && vibrationCoroutine == null) //headIsAbove > headIsInCPRPosition
            {
                vibrationCoroutine = StartCoroutine(TriggerVibration());
                Debug.Log("[CPRRecognizer] CPR 압박 인식 → 진동 시작");

                if (gameManager != null)
                    gameManager.TriggerCPR();
            }
            else if ((!isPressing || !headIsInCPRPosition) && vibrationCoroutine != null) //headIsAbove > headIsInCPRPosition
            {
                StopVibration();
                Debug.Log("[CPRRecognizer] 손 올림 → 진동 멈춤");
            }
        }
    }

    private IEnumerator TriggerVibration()
    {
        while (true)
        {
            OVRInput.SetControllerVibration(0.7f, 1.0f, OVRInput.Controller.LTouch);
            OVRInput.SetControllerVibration(0.7f, 1.0f, OVRInput.Controller.RTouch);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void StopVibration()
    {
        if (vibrationCoroutine != null)
        {
            StopCoroutine(vibrationCoroutine);
            vibrationCoroutine = null;
        }

        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignUIObjects();
    }

    void AssignUIObjects()
    {
        leftHand = GameObject.Find("LeftHandAnchor");
        rightHand = GameObject.Find("RightHandAnchor");
        headset = GameObject.Find("CenterEyeAnchor");
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    private void StartGlow()
    {
        if (chestMaterial != null && !isGlowing)
        {
            chestMaterial.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            chestMaterial.EnableKeyword("_EMISSION");
            chestMaterial.SetColor("_EmissionColor", Color.yellow * 3f);
            isGlowing = true;
        }
    }
    private void StopGlow()
    {
        if (chestMaterial != null && isGlowing)
        {
            chestMaterial.SetColor("_EmissionColor", baseColor);
            isGlowing = false;
        }
    }
}
