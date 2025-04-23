using System.Collections;
using UnityEngine;

public class Ending_Manager : MonoBehaviour
{
    public GameObject point_L;
    public GameObject fade_Black_Wall;     // ← 이거 페이드아웃 대상
    public GameObject ending_Cut_Image;
    public GameObject game_Clear_Image;
    public GameObject r_Click;

    private bool click_1 = true;
    private bool click_2 = false;
    private bool click_3 = false;
    private bool click_4 = false;

    public float fadeDuration = 0.5f; // 페이드 시간 (초)
    private Material mat;
    private Color originalColor;

    void Start()
    {
        click_1 = true;
        click_2 = false;
        click_3 = false;
        click_4 = false;

        point_L.SetActive(false);
        game_Clear_Image.SetActive(false);
        ending_Cut_Image.SetActive(false);
        r_Click.SetActive(true);


        if (fade_Black_Wall != null)
        {
            Renderer rend = fade_Black_Wall.GetComponent<Renderer>();
            if (rend != null)
            {
                mat = rend.material; // 이걸로 인스턴스화해서 직접 조절 가능
                originalColor = mat.color;
            }
        }
    }

    void Click_1_Me()
    {
        game_Clear_Image.SetActive(true);
        click_1 = false;
        click_2 = true;
    }

    void Click_2_Me()
    {
        game_Clear_Image.SetActive(false);
        ending_Cut_Image.SetActive(true);
        click_2 = false;
        click_3 = true;
    }

    void Click_3_Me()
    {
        StartCoroutine(FadeOut());
        
        click_3 = false;
        click_4 = true;
    }
    public void Clicker()
    {
        if (click_1)
        {
            Click_1_Me();
        }
        else if (click_2)
        {
            Click_2_Me();
        }
        else if (click_3)
        {
            Click_3_Me();
        }
        else if (click_4)
        {
            Debug.Log("모든 클릭 실행");
        }
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        fade_Black_Wall.SetActive(false); // 완전히 사라진 후 비활성화
        ending_Cut_Image.SetActive(false);
        point_L.SetActive(true);
        r_Click.SetActive(false);
    }
}
