using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public float playTimeMinutes = 1f;  // 게임 시간 (분 단위)
    public Text timerText;              // 타이머 UI 텍스트

    private float timer = 0f;           // 현재 시간
    public bool isTimeUp = false;       // 타이머 종료 여부

    private bool isStopped = true;   // 타이머 정지 여부

    public AudioClip effectSound;           // 한 번 재생할 사운드 클립
    private AudioSource audioSource;        // AudioSource 컴포넌트

    private bool hasStartedSound = false;

    // 전체 시간을 초 단위로 반환하는 프로퍼티
    public float TotalTimeInSeconds => playTimeMinutes * 60f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // 시작하면 타이머를 리셋함
        ResetTimer();
        isStopped = true;
    }

    void Update()
    {
        // 만약 시간이 끝나있다면 더이상 실행 x
        if (isTimeUp) return;
        if (isStopped) return;

        // 매 프레임 시간 감소
        timer -= Time.deltaTime;

        // 효과음 처음 시작할 때 1번만 루프 재생
        if (!hasStartedSound)
        {
            audioSource.clip = effectSound;
            audioSource.loop = true;
            audioSource.Play();
            hasStartedSound = true;
        }

        // 시간 텍스트 갱신
        timerText.text = $"구급차가 도착하기까지 : {Mathf.Max(timer, 0):F1}초";

        // 만약 타이머가 0 이하라면
        if (timer <= 0f)
        {
            // 타임 업 값 true
            isTimeUp = true;
            audioSource.Stop();
        }
    }

    public void StopTimer()
    {
        isStopped = true;
    }

    public bool IsTimeUp()
    {

        // 이 메서드가 실행되면 isTimeUp 값 반환
        return isTimeUp;
    }

    public void ResetTimer()
    {
        // 이 메서드가 실행되면 타이머리셋 후 타임 업 값 = false
        timer = TotalTimeInSeconds;
        isTimeUp = false;
        isStopped = false;

        hasStartedSound = false;
    }
}
