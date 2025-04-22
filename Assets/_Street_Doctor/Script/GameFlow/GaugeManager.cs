using UnityEngine;
using UnityEngine.UI;

public class GaugeManager : MonoBehaviour
{
    public Slider gaugeSlider;        // 게이지 UI 슬라이더
    public float decreaseSpeed = 5f;  // 게이지 감소 속도
    public float fillAmount = 10f;    // 게이지 채우기량

    public float fillMouseAmount = 0.2f;

    private bool isStopped = true;   // 게이지 정지 여부

    void Start()
    {
        // 시작 시 게이지를 최대치로 설정
        gaugeSlider.value = gaugeSlider.maxValue;
        ResetGauge();
        StopGauge();
    }

    void Update()
    {
        // 만약 멈춰있다면 실행안함
        if (isStopped) return;

        // 시간에 따라 게이지 감소
        gaugeSlider.value -= decreaseSpeed * Time.deltaTime;
    }

    // 게이지 감소 중지
    public void StopGauge()
    {
        isStopped = true;
    }

    // 게이지 초기화
    public void ResetGauge()
    {
        isStopped = false;
        gaugeSlider.value = gaugeSlider.maxValue;
    }

    // 게이지가 0 이하인지 확인
    public bool IsGaugeEmpty()
    {
        return gaugeSlider.value <= 0f;
    }

    // 트리거 입력 시 게이지 증가
    public void GaugeTrigger()
    {
        gaugeSlider.value += fillAmount;
        gaugeSlider.value = Mathf.Min(gaugeSlider.maxValue, gaugeSlider.value); // 최대치 제한
    }

    public void MouseTrigger()
    {
        gaugeSlider.value += fillMouseAmount;
        gaugeSlider.value = Mathf.Min(gaugeSlider.maxValue, gaugeSlider.value); // 최대치 제한
    }
}
