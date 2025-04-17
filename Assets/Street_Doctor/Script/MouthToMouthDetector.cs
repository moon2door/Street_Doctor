using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthToMouthDetector : MonoBehaviour
{
    public Transform centerEyeAnchor;       // OVRCameraRig/CenterEyeAnchor
    public float activationRadius = 0.2f;   // 입과 머리의 거리 허용 범위
    public float requiredTime = 3f;         // 머물러야 할 시간
    public float cooldownTime = 1f;         // 인식 간격 제한 (중복 방지 딜레이)

    private float timer = 0f;
    private bool isInZone = false;
    private float cooldownTimer = 0f;

    void Update()
    {
        float distance = Vector3.Distance(centerEyeAnchor.position, transform.position);

        // 쿨다운 적용: 이전 인식 직후엔 잠시 무시
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        if (distance < activationRadius)
        {
            if (!isInZone)
            {
                isInZone = true;
                timer = 0f;
                Debug.Log("📍 머리 위치 인식 시작");
            }

            timer += Time.deltaTime;

            if (timer >= requiredTime)
            {
                Debug.Log("✅ 인공호흡 성공!");
                // TODO: 효과 발생, 점수 증가, 애니메이션 등
                cooldownTimer = cooldownTime; // 중복 방지 대기
                isInZone = false;
            }
        }
        else
        {
            if (isInZone)
            {
                Debug.Log("❌ 중간에 벗어남, 타이머 리셋");
                isInZone = false;
                timer = 0f;
            }
        }
    }
}