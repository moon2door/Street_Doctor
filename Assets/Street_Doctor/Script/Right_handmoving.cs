using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Right_handmoving : MonoBehaviour
{
    LineRenderer myLR;
    GameObject right_hand;
    Ray ray;
    RaycastHit hit;

    public Transform playerRoot; // 플레이어 전체를 감싸는 루트(머리 기준)
    public float rotationSpeed = 45f; //playerRoot 의 시야 회전 속도 

    private GameObject grabbedObject = null;
    private bool isInitialized = false;

    void Start()
    {
        myLR = GetComponent<LineRenderer>();
        right_hand = GameObject.Find("RightHandAnchor");

        if (right_hand == null)
        {
            Debug.LogWarning("[Right_handmoving] RightHandAnchor가 씬에 없습니다. 이 스크립트는 비활성화됩니다.");
            enabled = false;
            return;
        }
        transform.position = right_hand.transform.position;
        transform.eulerAngles = right_hand.transform.eulerAngles;
        transform.parent = right_hand.transform;

        isInitialized = true;
    }

    void Update()
    {
        // 오른쪽 썸스틱으로 플레이어 시야 회전
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        
        if (Mathf.Abs(input.x) > 0.5f)
        {
            float rotationAmount = input.x * rotationSpeed * Time.deltaTime;
            playerRoot.Rotate(Vector3.up, rotationAmount);
        }

        // 오른손에서 레이 발사
        ray.origin = right_hand.transform.position;
        ray.direction = right_hand.transform.forward;
        myLR.SetPosition(0, ray.origin);
        myLR.SetPosition(1, ray.origin + ray.direction * 3);

        // 오른손에서발사한  레이케스트에 맞은 오브젝트가 있을때
        if (Physics.Raycast(ray, out hit, 3f))
        {
            myLR.startColor = Color.green;
            myLR.endColor = Color.green;
            myLR.SetPosition(1, hit.point);

            // 인덱스 트리거 : 문 상호작용
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("상호작용 시도: " + hit.collider.gameObject.name);
                DoorInteraction door = hit.collider.gameObject.GetComponentInParent<DoorInteraction>();
                if (door != null) door.OnInteract();
                else Debug.Log("문 오브젝트에서 DoorInteraction 스크립트를 찾을 수 없음");
            }

            // 핸드트리거 : 오브젝트 잡기
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                if (hit.collider != null && hit.collider.GetComponent<GrabObject>() != null)
                {
                    grabbedObject = hit.collider.gameObject;
                    grabbedObject.transform.position = right_hand.transform.position + ray.direction * 0.1f;
                    grabbedObject.transform.parent = right_hand.transform;
                    grabbedObject.transform.eulerAngles = Vector3.zero;
                }
            }
            // 잡고 있는 오브젝트가 붙이기 가능한 위치에 닿았고, 인덱스트리거를 눌렀을 때
            if (grabbedObject != null && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                if (hit.collider != null && hit.collider.GetComponent<AttachableSpot>() is AttachableSpot spot && spot.snapTransform != null)
                {
                    Debug.Log("붙이기 위치 감지됨: " + hit.collider.gameObject.name);

                    // 스냅 포지션 & 회전 적용
                    grabbedObject.transform.position = spot.snapTransform.position;
                    grabbedObject.transform.rotation = spot.snapTransform.rotation;
                    // 부모 설정 (붙이기 효과)
                    grabbedObject.transform.parent = spot.snapTransform;

                    StartCoroutine(ShortVibration(0.1f));//진동 세기
                    grabbedObject = null; // 손에서 해제                    
                }
            }
        }
        else  // 레이캐스트 실패 시 빨간색 라인
        {
            myLR.startColor = Color.red;
            myLR.endColor = Color.red;
        }

        // 핸드트리거를 뗐을때 오브젝트 놓기(제자리 복귀)
        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            if (grabbedObject != null)
            {
                grabbedObject.transform.parent = null;
                GameObject location = GameObject.Find(grabbedObject.name + "_");// 잡은 오브젝트 이름 + "_" 형식의 복귀 위치 탐색
                if (location != null)
                {
                    grabbedObject.transform.eulerAngles = Vector3.zero;
                    grabbedObject.transform.parent = location.transform;
                    grabbedObject.transform.localPosition = Vector3.zero;
                }
                grabbedObject = null;
            }
        }
    }
    IEnumerator ShortVibration(float duration)
    {
        // 진동 시작: 강도 0.5, 주파수 0.5
        OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
        // duration 초 기다림
        yield return new WaitForSeconds(duration);
        // 진동 정지
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }
}
