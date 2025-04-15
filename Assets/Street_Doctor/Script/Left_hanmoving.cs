using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Left_handmoving : MonoBehaviour
{
    LineRenderer myLR;
    GameObject left_hand;
    Ray ray;
    RaycastHit hit;
    
    public float moveSpeed = 2.0f;  // 이동 속도
    public Transform playerRoot;   // 플레이어 전체를 감싸는 루트(머리 기준)

    private GameObject grabbedObject = null;

    void Start()
    {
        myLR = GetComponent<LineRenderer>();
        left_hand = GameObject.Find("LeftHandAnchor");

        transform.position = left_hand.transform.position;
        transform.eulerAngles = left_hand.transform.eulerAngles;
        transform.parent = left_hand.transform;
    }

    void Update()
    {
        //  왼손 썸스틱으로 플레이어 이동
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector3 moveDir = left_hand.transform.right * input.x + left_hand.transform.forward * input.y;
        moveDir.y = 0f; // 수직 이동 방지

        if (playerRoot != null)
            playerRoot.position += moveDir * moveSpeed * Time.deltaTime;

        // 왼손에서 레이 발사
        ray.origin = left_hand.transform.position;
        ray.direction = left_hand.transform.forward;
        myLR.SetPosition(0, ray.origin);
        myLR.SetPosition(1, ray.origin + ray.direction * 3);

        // 왼손에서 발사된 레이케스트 충돌시
        if (Physics.Raycast(ray, out hit, 3f))
        {
            myLR.startColor = Color.green;
            myLR.endColor = Color.green;
            myLR.SetPosition(1, hit.point);

            // 인덱스 트리거 : 문 상호작용
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                Debug.Log("상호작용 시도: " + hit.collider.gameObject.name);
                DoorInteraction door = hit.collider.gameObject.GetComponentInParent<DoorInteraction>();
                if (door != null) door.OnInteract();
                else Debug.Log("문 오브젝트에서 DoorInteraction 스크립트를 찾을 수 없음");
            }

            //  핸드트리거: 오브젝트 잡기
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
            {
                if (hit.collider != null && hit.collider.GetComponent<GrabObject>() != null)
                {
                    grabbedObject = hit.collider.gameObject;
                    grabbedObject.transform.position = left_hand.transform.position + ray.direction * 0.1f;
                    grabbedObject.transform.parent = left_hand.transform;
                    grabbedObject.transform.eulerAngles = Vector3.zero;
                }
            }
            // 잡은 오브젝트가 붙이기 가능한 위치에 닿았고, 인덱스트리거를 누른 경우
            if (grabbedObject != null && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                if (hit.collider != null && hit.collider.GetComponent<AttachableSpot>() != null)
                {
                    Debug.Log("붙이기 위치 감지됨: " + hit.collider.gameObject.name);

                    grabbedObject.transform.parent = hit.collider.transform;
                    grabbedObject.transform.localPosition = Vector3.zero;
                    grabbedObject.transform.localRotation = Quaternion.identity;

                    grabbedObject = null; // 손에서 놓기
                }
            }
        }
        else // 레이 충돌 없음: 빨간색 라인
        {
            myLR.startColor = Color.red;
            myLR.endColor = Color.red;
        }

        // 핸드트리거를 뗐을 때 오브젝트 놓기(제자리 복귀)
        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            if (grabbedObject != null)
            {
                grabbedObject.transform.parent = null;
                GameObject location = GameObject.Find(grabbedObject.name + "_"); // "오브젝트이름_" 형태의 기준 위치 찾기
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
}

