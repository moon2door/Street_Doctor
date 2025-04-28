using UnityEngine;

public class MouthToMouthDetector : MonoBehaviour
{
    public Transform centerEyeAnchor;       // 플레이어 머리 (OVRCameraRig의 CenterEyeAnchor)
    public GameManager gameManager;

    public float activationRadius = 0.2f;   // 머리와 대상 간 거리 조건
    public float requiredTime = 3f;         // 머무를 시간
    public float cooldownTime = 1f;         // 성공 후 재인식까지 대기시간

    private float timer = 0f;
    private bool isInZone = false;
    private float cooldownTimer = 0f;

    public AudioClip kissSound;
    private AudioSource audioSource;        // AudioSource 컴포넌트

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (centerEyeAnchor == null)
        {
            GameObject eye = GameObject.Find("CenterEyeAnchor");
            if (eye != null)
            {
                centerEyeAnchor = eye.transform;
                Debug.Log("[MouthToMouth] CenterEyeAnchor 자동 할당됨");
            }
            else
            {
                Debug.LogWarning("[MouthToMouth] CenterEyeAnchor를 찾을 수 없습니다.");
            }
        }

        gameManager = GameObject.FindObjectOfType<GameManager>();

    }

    void Update()
    {
        if (centerEyeAnchor == null) return;

        float distance = Vector3.Distance(centerEyeAnchor.position, transform.position);

        // 쿨다운 타이머 중엔 무시
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        if (distance < activationRadius)
        {
            if (!isInZone)
            {
                isInZone = true;
                timer = 0f;

                Debug.Log("[MouthToMouth] 머리 위치 인식 시작");

                if (!gameManager.isCPRPhase)
                {
                    audioSource.PlayOneShot(kissSound);
                }
            }

            timer += Time.deltaTime;
            
            

            if (timer >= requiredTime)
            {
                Debug.Log("[MouthToMouth] 인공호흡 성공!");

                if (gameManager != null)
                    gameManager.TriggerBreath();

                cooldownTimer = cooldownTime;
                isInZone = false;
            }
        }
        else
        {
            if (isInZone)
            {
                Debug.Log("[MouthToMouth] 중간에 벗어남 → 타이머 초기화");
                isInZone = false;
                timer = 0f;
            }
        }
    }
}
