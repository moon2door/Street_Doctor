using System.Collections;
using UnityEngine;

public class CPRRecognizer : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject headset;
    public float chestY = 0.5f;
    public float pressDepth = 0.015f;

    private bool leftIn = false;
    private bool rightIn = false;
    public bool isCPRActive { get; private set; } = false;

    private Coroutine vibrationCoroutine = null;
    private GaugeManager gaugeManager;

    private void Start()
    {
        chestY = transform.position.y + 0.03f;
        GameObject gm = GameObject.Find("GaugeManager");
        if (gm != null)
            gaugeManager = gm.GetComponent<GaugeManager>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Left_Hand") leftIn = true;
        if (other.name == "Right_Hand") rightIn = true;

        if (leftIn && rightIn)
            isCPRActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Left_Hand") leftIn = false;
        if (other.name == "Right_Hand") rightIn = false;

        StopVibration();

        if (!leftIn || !rightIn)
            isCPRActive = false;
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
                Debug.Log("CPR 압박 인식 → 진동 시작");

                if (gaugeManager != null)
                    gaugeManager.GaugeTrigger();
            }
            else if ((!isPressing || !headIsAbove) && vibrationCoroutine != null)
            {
                StopVibration();
                Debug.Log("⬆️ 손 올림 → 진동 멈춤");
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
}

