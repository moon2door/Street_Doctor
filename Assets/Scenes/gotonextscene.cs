using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gotonextscene : MonoBehaviour
{
    public Image fadeImage;          // 검정색 UI 이미지 연결
    public float delay = 10f;        // 몇 초 후에 페이드 시작할지
    public float fadeDuration = 2f;  // 페이드 되는 시간

    void Start()
    {
        StartCoroutine(FadeOutAfterDelayCoroutine());
    }

    private System.Collections.IEnumerator FadeOutAfterDelayCoroutine()
    {
        // 시작 시 알파 0으로 설정 (투명)
        Color color = fadeImage.color;
        fadeImage.color = new Color(color.r, color.g, color.b, 0f);

        // 10초 대기
        yield return new WaitForSeconds(delay);

        // 페이드 아웃 시작
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // 완전히 어두워진 후 알파값 확정
        fadeImage.color = new Color(color.r, color.g, color.b, 1f);
        SceneManager.LoadScene("C");
    }
}
