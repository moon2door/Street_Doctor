using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class right_hand_script : MonoBehaviour
{

    GameObject righthand;
    public Image fadeImage;
    public float fadeDuration = 4f;
    public float delayBeforeSceneChange = 7f;

    bool isFading = false;
    float fadeTimer = 0f;
    bool triggerPressed = false;

    void Start()
    {
        righthand = GameObject.Find("RightHandAnchor");
        transform.position = righthand.transform.position;
        transform.parent = righthand.transform;

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    void Update()
    {

        if (!triggerPressed && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            triggerPressed = true;
            isFading = true;
            fadeTimer = 0f;
            Debug.Log("트리거 눌림! 페이드 시작");

            StartCoroutine(SceneChangeAfterDelay(delayBeforeSceneChange));
        }


        if (isFading && fadeImage != null)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, fadeTimer / fadeDuration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);

            if (fadeTimer >= fadeDuration)
            {

                isFading = false;
            }
        }
    }

    IEnumerator SceneChangeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("B");
    }
}

