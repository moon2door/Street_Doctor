using UnityEngine;

public class CameraSync : MonoBehaviour
{
    public Transform playerRoot;  // 따라갈 루트

    private Vector3 initialOffset;

    void Start()
    {
        if (playerRoot == null)
        {
            Debug.LogWarning("CameraSync: playerRoot가 연결되지 않았습니다.");
            enabled = false;
            return;
        }

        initialOffset = transform.position - playerRoot.position;
    }

    void LateUpdate()
    {
        transform.position = playerRoot.position + initialOffset;
    }
}
