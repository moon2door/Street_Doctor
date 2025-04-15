using UnityEngine;
using UnityEngine.UI;

public class backgroundcontrol : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 5f;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        Color color = fadeImage.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }


        fadeImage.gameObject.SetActive(false);
    }
}
