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
    public float pressDepth = 0.015f;

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

            bool isPressing = chestDepth > pressDepth - 0.005f;

            Vector3 chestPos = transform.position;
            Vector3 handsCenter = (leftHand.transform.position + rightHand.transform.position) * 0.5f;
            Vector3 handToChestDir = (chestPos - handsCenter).normalized;
            Vector3 headDir = (chestPos - headset.transform.position).normalized;
            float alignmentDot = Vector3.Dot(handToChestDir, headDir);
            bool headIsAbove = alignmentDot > 0.1f;

            if (isPressing && headIsAbove && vibrationCoroutine == null)
            {
                vibrationCoroutine = StartCoroutine(TriggerVibration());
                Debug.Log("[CPRRecognizer] CPR 압박 인식 → 진동 시작");

                if (gameManager != null)
                    gameManager.TriggerCPR();
            }
            else if ((!isPressing || !headIsAbove) && vibrationCoroutine != null)
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
