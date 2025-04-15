using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RH__1 : MonoBehaviour
{
    GameObject right_H;                  // 오른손 앵커

    void Start()
    {
        // 오른손 위치에 오브젝트 붙이기
        right_H = GameObject.Find("RightHandAnchor");
        transform.position = right_H.transform.position;
        transform.parent = right_H.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            SceneManager.LoadScene("B_Scene_Loading"); // B 씬 (로딩 씬)으로 이동
        }
    }
}
