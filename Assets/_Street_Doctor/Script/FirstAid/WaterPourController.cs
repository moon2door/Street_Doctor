using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaterPourController : MonoBehaviour
{
    public Transform pourPoint;             // 입구 위치
    public ParticleSystem waterParticle;    // 파티클 시스템
    public Transform targetZone;            // 물 받는 위치 기준점
    public Text completeMessage;            // 완료 메시지 UI

    public float pourAngleThreshold = 0.7f; // 얼마나 기울어야 붓는걸로 판단 (dot 기준)
    public float pourDistance = 1f;       // targetZone과 거리 제한
    public float totalPourTime = 10f;       // 전체 물 붓는 시간   

    private float pourTimer = 0f;
    private bool isComplete = false;
    //private bool isPouring = false;

    void Update()
    {
        if (isComplete) return;

        Vector3 pourDir = pourPoint.up; // 입구가 향하는 방향
        float dot = Vector3.Dot(pourDir, Vector3.down); // 아래쪽과 얼마나 일치하는지
        float distToTarget = Vector3.Distance(pourPoint.position, targetZone.position);// 거리 조건

        bool pouringCondition = dot > pourAngleThreshold; // 파티클은 기울기만 만족하면 계속 나와야 함

        // 파티클은 기울기만 만족하면 계속 나와야 함
        if (pouringCondition)
        {
            if (!waterParticle.isPlaying)
            {
                waterParticle.Play();
                Debug.Log("💧 물 붓기 시작");
            }

            if (distToTarget < pourDistance) // 타겟 존 근처일 때만 타이머 작동
            {
                pourTimer += Time.deltaTime;

                if (pourTimer >= totalPourTime)
                {
                    isComplete = true;
                    if (completeMessage != null)
                        completeMessage.gameObject.SetActive(true);
                   Debug.Log(" 물 붓기 완료!");
                }
            }
        }
        else
        {   // 기울기 안됐으면 파티클 멈춤
            if (waterParticle.isPlaying)
            {
                waterParticle.Stop();
                Debug.Log("⛔ 물 멈춤 (기울기 부족)");
            }
        }
    }
}