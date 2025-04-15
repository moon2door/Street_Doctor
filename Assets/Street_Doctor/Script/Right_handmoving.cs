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

    void Start()
    {
        myLR = GetComponent<LineRenderer>();
        right_hand = GameObject.Find("RightHandAnchor");

        transform.position = right_hand.transform.position;
        transform.eulerAngles = right_hand.transform.eulerAngles;
        transform.parent = right_hand.transform;
    }

    void Update()
    {
        // 오른쪽 썸스틱으로 화면회전
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (Mathf.Abs(input.x) > 0.2f)
        {
            float rotationAmount = input.x * rotationSpeed * Time.deltaTime;
            playerRoot.Rotate(Vector3.up, rotationAmount);
        }

        // 레이캐스트
        ray.origin = right_hand.transform.position;
        ray.direction = right_hand.transform.forward;
        myLR.SetPosition(0, ray.origin);
        myLR.SetPosition(1, ray.origin + ray.direction * 3);

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
        }
        else
        {
            myLR.startColor = Color.red;
            myLR.endColor = Color.red;
        }

        // 오브젝트 놓기
        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
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