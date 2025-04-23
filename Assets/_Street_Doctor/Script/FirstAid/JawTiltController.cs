using UnityEngine;

public class JawTiltController : MonoBehaviour
{
    public Transform head;          // Head 본
    public Transform headEnd;       // 손 위치 감지용
    public Transform rightHand;     // 오른손
    public Transform leftHand;      // 왼손
    public float activationDistance = 0.1f;
    public float tiltAngle = -15f;  // 뒤로 젖히는 각도 (음수면 뒤로)
    public float rotationSpeed = 2f;    // 고개 젖히는 속도 (값이 클수록 빠름)
    public float moveAmountZ = 0.03f;   // Z 이동값
    private bool isTilting = false;
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    public bool hasActivated = false;

    void Update()
    {
        if (hasActivated || head == null || headEnd == null || (rightHand == null && leftHand == null))
            return;

        float rightDist = rightHand != null ? Vector3.Distance(rightHand.position, headEnd.position) : float.MaxValue;
        float leftDist = leftHand != null ? Vector3.Distance(leftHand.position, headEnd.position) : float.MaxValue;
        if (rightDist < activationDistance || leftDist < activationDistance)
        {
            Debug.Log("🟢 기도 확보 조건 충족 → 고개 천천히 젖히기 시작");

            // 목표 회전 및 위치 저장
            targetRotation = head.localRotation * Quaternion.Euler(tiltAngle, 0f, 0f);
            targetPosition = head.localPosition + new Vector3(0f, 0f, moveAmountZ);
            isTilting = true;
        }

        if (isTilting)
        {
            head.localRotation = Quaternion.Lerp(head.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
            head.localPosition = Vector3.Lerp(head.localPosition, targetPosition, Time.deltaTime * rotationSpeed);

            if (Quaternion.Angle(head.localRotation, targetRotation) < 0.5f && Vector3.Distance(head.localPosition, targetPosition) < 0.001f)
            {
                isTilting = false;
                hasActivated = true;
            }
        }
    }
}