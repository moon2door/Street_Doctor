using System.Collections;
using UnityEngine;

public class CPRRecognizer : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject headset;
    public float chestY = 0.5f; // 심장(트리거)의 Y 위치
    public float pressDepth = 0.015f; // 눌려야 할 최소 깊이 (1.5cm)
   
    private bool leftIn = false;
    private bool rightIn = false;
    public bool isCPRActive { get; private set; } = false;//불값 변수선언(UI선언이나 음성피드백설정가능,시간측정가능)

    private Coroutine vibrationCoroutine = null;

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
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Left_Hand") leftIn = false;
        if (other.name == "Right_Hand") rightIn = false;

        StopVibration(); // 한 손이라도 빠지면 진동 멈춤

        if (!leftIn || !rightIn)
        {
            isCPRActive = false;
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

            //Debug.Log($"[CPR 검사] chestDepth={chestDepth:F3}, pressDepth={pressDepth}, alignmentDot=미계산");

            if (chestDepth < pressDepth)
                Debug.Log("❌ 눌림 깊이가 부족 → isPressing = false");

            bool isPressing = chestDepth >pressDepth - 0.005f; // 약간 덜 눌려도 OK

            //Debug.Log($"✅ isPressing = {isPressing}");

            Vector3 chestPos = transform.position; // 트리거(심장) 위치
            Vector3 handsCenter = (leftHand.transform.position + rightHand.transform.position) * 0.5f;
            Vector3 handToChestDir = (chestPos - handsCenter).normalized;

            Vector3 headDir = (chestPos - headset.transform.position).normalized;

            float alignmentDot = Vector3.Dot(handToChestDir, headDir);
            //Debug.Log($"🧭 정렬 dot: {alignmentDot:F3}");
            if (alignmentDot < 0.17f)
                Debug.Log("❌ 헤드 정렬 부족 → headIsAbove = false");

            bool headIsAbove = alignmentDot > 0.1f; // 대략 60도 이상 수직에 있을 때


            if (isPressing && headIsAbove && vibrationCoroutine == null)
            {
                vibrationCoroutine = StartCoroutine(TriggerVibration());
                Debug.Log(" CPR 압박 인식 → 진동 시작");
            }
            else if ((!isPressing || !headIsAbove) && vibrationCoroutine != null)
            {
                StopVibration();
                Debug.Log("⬆️ 손 올림 → 진동 멈춤");
            }
        }
        //else
        //{
        //    Debug.Log($"❌ 조건 미충족 → leftIn={leftIn}, rightIn={rightIn}");
        //}
    }
    private IEnumerator TriggerVibration()//진동 시작
    {
        while (true)
        {
            OVRInput.SetControllerVibration(0.7f, 1.0f, OVRInput.Controller.LTouch);
            OVRInput.SetControllerVibration(0.7f, 1.0f, OVRInput.Controller.RTouch);
            yield return new WaitForSeconds(0.1f); // 0.1초만 강하게
        }
    }
        private void StopVibration()//진동 멈춤
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
