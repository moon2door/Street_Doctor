using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedHead : MonoBehaviour
{
    public Transform centerEyeAnchor; // OVRCameraRig 안의 CenterEyeAnchor
    public Transform fixedHeadTransform; // 고정 시선 위치로 사용할 빈 오브젝트
    public float fixedY = 1.6f; // 고정할 Y값 (성인 평균 눈높이 약 160cm)

    void Update()
    {
        if (centerEyeAnchor == null || fixedHeadTransform == null)
            return;

        Vector3 eyePos = centerEyeAnchor.position;
        fixedHeadTransform.position = new Vector3(eyePos.x, fixedY, eyePos.z);
        fixedHeadTransform.rotation = centerEyeAnchor.rotation;
    }
}
