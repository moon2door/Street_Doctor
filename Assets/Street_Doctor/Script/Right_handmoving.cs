using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Right_handmoving : MonoBehaviour
{
    LineRenderer myLR;
    GameObject right_hand;
    Ray ray;
    RaycastHit hit;

    public Transform playerRoot;         // 회전의 중심 (PlayerRoot 또는 OVRCameraRig 부모)
    public float rotationSpeed = 45f;    // 초당 회전 속도 (도 단위)
    //Start is called before the first frame update
    void Start()
    {
        myLR = GetComponent<LineRenderer>();
        right_hand = GameObject.Find("RightHandAnchor");

        transform.position = right_hand.transform.position;
        transform.eulerAngles = right_hand.transform.eulerAngles;
        transform.parent = right_hand.transform;
    }

    //Update is called once per frame
    void Update()
    {
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick); // 오른손 썸스틱

        if (Mathf.Abs(input.x) > 0.2f) // 좌우 입력 감지 (Deadzone 설정)
        {
            float rotationAmount = input.x * rotationSpeed * Time.deltaTime;
            playerRoot.Rotate(Vector3.up, rotationAmount);
        }
        ray.origin = right_hand.transform.position;
        ray.direction = right_hand.transform.forward;
        myLR.SetPosition(0, ray.origin);
        myLR.SetPosition(1, ray.origin + ray.direction * 5);

        if (Physics.Raycast(ray, out hit, 5f))
        {
            myLR.startColor = Color.green;
            myLR.endColor = Color.green;
            myLR.SetPosition(1, hit.point);

            //  인덱스트리거: 상호작용 (예: 문열기)
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Debug.Log("상호작용 시도: " + hit.collider.gameObject.name);
                DoorInteraction door = hit.collider.gameObject.GetComponentInParent<DoorInteraction>();
                if (door != null)
                {
                    door.OnInteract();
                }
                else
                {
                    Debug.Log("문 오브젝트에서 DoorInteraction 스크립트를 찾을 수 없음");
                }
            }
            //  핸드트리거: 잡기
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                if (hit.collider != null)
                {
                    hit.collider.gameObject.transform.position = right_hand.transform.position + ray.direction * 0.1f;
                    hit.collider.gameObject.transform.parent = right_hand.transform;
                    hit.collider.gameObject.transform.eulerAngles = Vector3.zero;
                }
            }

            //  핸드트리거 해제: 놓기
            if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                if (hit.collider != null)
                {
                    hit.collider.gameObject.transform.parent = null;
                    GameObject location = GameObject.Find(hit.collider.gameObject.name + "_");
                    hit.collider.gameObject.transform.eulerAngles = Vector3.zero;
                    hit.collider.gameObject.transform.parent = location.transform;
                    hit.collider.gameObject.transform.localPosition = Vector3.zero;
                }
            }
        }
        else
        {
            myLR.startColor = Color.red;
            myLR.endColor = Color.red;
        }
    }
}
