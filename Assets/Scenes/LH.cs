using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LH : MonoBehaviour
{
    GameObject Left_H;                  // 왼손 앵커
    public GameManager gameManager;

    void Start()
    {
        // 왼손 위치에 오브젝트 붙이기
        Left_H = GameObject.Find("LeftHandAnchor");
        transform.position = Left_H.transform.position;
        transform.parent = Left_H.transform;

        GameObject gmObj = GameObject.Find("GameManager");
        if (gmObj != null)
        {
            gameManager = gmObj.GetComponent<GameManager>();
        }
        else
        {
            Debug.LogWarning("GameManager 오브젝트를 찾지 못했습니다. LH에서");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 왼손 트리거로 게임 재시작
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            if (gameManager != null)
            {
                gameManager.RestartGame();
            }
            else
            {
                Debug.LogWarning("LH.cs에서 gameManager가 null입니다");
            }
        }
    }
}
