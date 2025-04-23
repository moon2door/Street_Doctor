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
       
    void Start()
    {
        // 라인렌더러 및 오른손 위치 초기화
        myLR = GetComponent<LineRenderer>();
        right_H = GameObject.Find("RightHandAnchor");

        transform.position = right_H.transform.position;
        transform.eulerAngles = right_H.transform.eulerAngles;
        transform.parent = right_H.transform;

    }

    void Update()
    {
       
        HandlePlayerRotation();   // 플레이어 회전 처리
      
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

}
