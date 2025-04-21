using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LH_Phone : MonoBehaviour
{
    GameObject left_Phone;

    void Start()
    {
        left_Phone = GameObject.Find("LeftHandAnchor");

        // 부모로 설정
        transform.parent = left_Phone.transform;

        // 위치 조정: 왼손 위치 + x 방향으로 0.054f
        transform.localPosition = new Vector3(0.054f, 0f, 0f);

        // 회전 조정: 현재 회전에 + (y: 90, z: 45)
        Quaternion additionalRotation = Quaternion.Euler(0, 90, 45);
        transform.localRotation = additionalRotation;
    }
}
