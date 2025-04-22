using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRCameraRigManager : MonoBehaviour
{
    private static bool isInstantiated = false;

    void Awake()
    {
        if (!isInstantiated)
        {
            GameObject ovrCamPrefab = Resources.Load<GameObject>("PlayerRoot");

            if (ovrCamPrefab != null)
            {
                GameObject camInstance = Instantiate(ovrCamPrefab);
                DontDestroyOnLoad(camInstance);
                isInstantiated = true;
                Debug.Log("✅ OVRCameraRig 최초 생성 완료");
            }
            else
            {
                Debug.LogError("❌ Resources 폴더에 OVRCameraRig 프리팹이 없습니다.");
            }
        }
        else
        {
            Debug.Log("⚠️ OVRCameraRig 이미 생성됨");
        }

        Destroy(this.gameObject); // 매니저는 필요 없으니 파괴
    }
}
