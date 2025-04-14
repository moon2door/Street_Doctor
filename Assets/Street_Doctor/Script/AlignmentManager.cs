using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentManager : MonoBehaviour
{
    public GameObject rightHand;
    public GameObject leftHand;
    public GameObject headset;
    public GameObject targetObject;

    private bool isAligned = false;

    void Update()
    {
        Vector3 rightPos = rightHand.transform.position;
        Vector3 leftPos = leftHand.transform.position;
        Vector3 headPos = headset.transform.position;

        Vector3 vector1 = (leftPos - rightPos).normalized;
        Vector3 vector2 = (headPos - leftPos).normalized;

        float alignment = Vector3.Dot(vector1, vector2);

        if (Mathf.Abs(alignment) > 0.98f)
        {
            if (!isAligned)
            {
                Debug.Log(" 손과 머리가 세로 일직선입니다. 진동 시작");
                StartCoroutine(VibrateBothHands(2f)); // 진동 2초
                isAligned = true;
            }
        }
        else
        {
            isAligned = false;
            Debug.Log(" 일직선이 아님");
        }
    }    IEnumerator VibrateBothHands(float duration)
    {
        OVRInput.SetControllerVibration(1.0f, 1.0f, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(1.0f, 1.0f, OVRInput.Controller.RTouch);
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }
}