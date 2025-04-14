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

        // 레이캐스트
        ray.origin = left_hand.transform.position;
        ray.direction = left_hand.transform.forward;
        myLR.SetPosition(0, ray.origin);
        myLR.SetPosition(1, ray.origin + ray.direction * 3);

        if (Physics.Raycast(ray, out hit, 3f))
        {
            myLR.startColor = Color.green;
            myLR.endColor = Color.green;
            myLR.SetPosition(1, hit.point);

            // 문 상호작용
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                Debug.Log("상호작용 시도: " + hit.collider.gameObject.name);
                DoorInteraction door = hit.collider.gameObject.GetComponentInParent<DoorInteraction>();
                if (door != null) door.OnInteract();
                else Debug.Log("문 오브젝트에서 DoorInteraction 스크립트를 찾을 수 없음");
            }

            //  핸드트리거: 잡기
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
        }
        else
        {
            myLR.startColor = Color.red;
            myLR.endColor = Color.red;
        }

        // 오브젝트 놓기
        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            if (grabbedObject != null)
            {
                grabbedObject.transform.parent = null;
                GameObject location = GameObject.Find(grabbedObject.name + "_");
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

