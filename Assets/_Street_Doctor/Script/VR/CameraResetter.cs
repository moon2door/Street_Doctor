using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraResetter : MonoBehaviour
{
    public Vector3 resetPosition = Vector3.zero;
    public Vector3 resetRotation = Vector3.zero; // EulerAngles
    public GameObject ovrRig;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ovrRig = GameObject.Find("PlayerRoot(Clone)");

        if (ovrRig != null)
        {
            ovrRig.transform.position = resetPosition;
            ovrRig.transform.eulerAngles = resetRotation;
            Debug.Log($"🔄 OVR 카메라 위치 초기화: {resetPosition}");
        }
        else
        {
            Debug.LogWarning("❌ OVRCameraRig 찾을 수 없음");
        }
    }
}
