using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BillboardCanvas : MonoBehaviour
{
    public Transform target; // 보통 CenterEyeAnchor

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (target == null) return;

        // 플레이어 방향으로 회전하되, Y축만 따라가도록 제한 (UI가 기울어지지 않게)
        Vector3 direction = target.position - transform.position;
        direction.y = 0; // Y축 회전만 남김

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation * Quaternion.Euler(0, 180f, 0); // Y축 기준으로 180도 반전
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { 
            AssignUIObjects();
    }

    void AssignUIObjects()
    {
        target = GameObject.Find("CenterEyeAnchor")?.transform;
    }
}
