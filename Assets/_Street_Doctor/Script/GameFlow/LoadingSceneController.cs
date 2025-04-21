using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    public CanvasGroup loadingCanvasGroup; // 페이드 아웃용
    public GameManager gameManager;

    void Start()
    {
        GameObject gmObj = GameObject.Find("GameManager");
        if (gmObj != null)
        {
            gameManager = gmObj.GetComponent<GameManager>();
        }

        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        StartCoroutine(gameManager.FadeReturn());

        string sceneToLoad = DontDestroy.nextSceneName;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        float minLoadTime = 2f;
        float elapsed = 0f;

        while (asyncLoad.progress < 0.9f || elapsed < minLoadTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ✅ 먼저 페이드 아웃 실행
        yield return StartCoroutine(FadeOutLoadingUI());

        // ✅ UI 사라진 뒤 씬 활성화
        asyncLoad.allowSceneActivation = true;

        // ✅ 다음 프레임에 로딩씬 제거 (씬이 완전히 전환된 이후)
        yield return null;
        SceneManager.UnloadSceneAsync("2_Load");
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
