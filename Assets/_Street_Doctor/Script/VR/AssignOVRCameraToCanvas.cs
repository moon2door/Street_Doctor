using UnityEngine;
using System.Collections;

public class AssignOVRCameraToCanvas : MonoBehaviour
{
    private Canvas canvas;
    public bool isAssigned = false;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            if (canvas.renderMode != RenderMode.ScreenSpaceCamera)
            {
                Debug.LogWarning("ScreenSpaceCamera 캔버스가 아닙니다.");
            }
            else
            {
                Debug.LogWarning("canvus만 null 입니다.");
            }

            return;
        }

        StartCoroutine(AssignCameraWhenReady());
    }

    IEnumerator AssignCameraWhenReady()
    {
        while (!isAssigned)
        {
            GameObject cameraObj = GameObject.Find("CenterEyeAnchor");

            if (cameraObj != null)
            {
                Camera cam = cameraObj.GetComponent<Camera>();
                if (cam != null)
                {
                    canvas.worldCamera = cam;
                    Debug.Log("캔버스에 OVR 카메라 할당 완료");
                    isAssigned = true;
                    yield break;
                }
            }
            Debug.LogWarning("캔버스에 OVR 카메라 할당 못하는 중..");
            yield return new WaitForSeconds(0.2f); // 0.2초 간격으로 재시도
        }
    }
}
