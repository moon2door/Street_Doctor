using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    public Slider loadingBar;
    public CanvasGroup loadingCanvasGroup; // 페이드 아웃용

    void Start()
    {
        StartCoroutine(LoadCScene());
    }

    IEnumerator LoadCScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("C_Scene_2", LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            loadingBar.value = asyncLoad.progress;
            yield return null;
        }

        loadingBar.value = 1f;
        yield return new WaitForSeconds(0.5f);

        asyncLoad.allowSceneActivation = true;

        // 로딩 완료 후 페이드 아웃
        yield return StartCoroutine(FadeOutLoadingUI());

        // 로딩 완료된 후 B 씬 제거
        SceneManager.UnloadSceneAsync("B_Scene_Loading");
    }

    IEnumerator FadeOutLoadingUI()
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            loadingCanvasGroup.alpha = 1f - (elapsed / duration);
            yield return null;
        }

        loadingCanvasGroup.alpha = 0f;
    }
}
