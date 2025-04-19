using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandageWrapDetector : MonoBehaviour
{
    public Transform rightHand;       // 오른손 트래킹 오브젝트
    public Transform targetArm;       // 환자의 팔 Transform
    public GameObject bandageObject;  // 붙일 붕대 오브젝트
    public Transform attachPoint;     // 붕대가 부착될 위치 기준
    AudioSource Baudio;
    public AudioClip wrapSound;// 붕대감기는 소리

    public float wrapThreshold = 720f; // 총 누적 회전량 (예: 2바퀴)

    private Vector3 lastDirection = Vector3.zero;
    private float totalRotation = 0f;
    private bool isAttached = false;
    void Start()
    {
        Baudio = GetComponent<AudioSource>();

        // rightHand가 없으면 이 씬에서는 동작하지 않도록 비활성화
        if (rightHand == null)
        {
            Debug.LogWarning("[BandageWrapDetector] rightHand가 할당되지 않아 동작하지 않습니다.");
            enabled = false;
        }
    }
    void Update()
    {
        if (isAttached) return;

        Vector3 currentDir = (rightHand.position - targetArm.position).normalized;

        if (lastDirection != Vector3.zero)
        {
            float angle = Vector3.Angle(lastDirection, currentDir);
            totalRotation += angle;

            // 테스트 로그
            Debug.Log($"[Bandage Wrap] 누적 회전량: {totalRotation:F1}도");
        }

        lastDirection = currentDir;

        if (totalRotation >= wrapThreshold)
        {
            AttachBandage();
            isAttached = true;
        }
    }

    void AttachBandage()
    {
        bandageObject.transform.SetParent(attachPoint);
        bandageObject.transform.localPosition = Vector3.zero;
        bandageObject.transform.localRotation = Quaternion.identity;

        Debug.Log("[Bandage Wrap] 붕대가 팔에 부착되었습니다!");
        // TODO: 부가 연출 (사운드/이펙트 등) 추가 가능
        if (wrapSound != null) Baudio.PlayOneShot(wrapSound);
    }
}
