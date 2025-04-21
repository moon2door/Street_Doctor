using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class KeypadTouchInput : MonoBehaviour
{
    public Text inputText;                 // 전화번호 출력용 텍스트
    public CallingUIManager callUIManager;   // 신고 UI 제어
    private string currentNumber = "";

    private bool canTouch = true;         // 터치 딜레이 컨트롤

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnTriggerEnter(Collider other)
    {
        if (!canTouch) return;

        string tag = other.tag;

        if (tag.StartsWith("Key"))
        {
            string key = tag.Replace("Key", ""); // Key1 → 1
            currentNumber += key;
            inputText.text = currentNumber;
            Debug.Log($"🔢 입력된 숫자: {key}");
            StartCoroutine(TouchDelay());
        }
        else if (tag == "CallButton")
        {
            StartCoroutine(TouchDelay());

            if (currentNumber == "119")
            {
                Debug.Log("✅ 119 신고 완료!");
                callUIManager.ShowCompleteMessage();
                gameObject.SetActive(false); // 이후 터치 비활성화
            }
            else
            {
                Debug.Log("❌ 잘못된 번호 입력");
                callUIManager.ShowErrorMessage();
                StartCoroutine(BlinkText());
            }
        }
    }

    IEnumerator TouchDelay()
    {
        canTouch = false;
        yield return new WaitForSeconds(0.5f);
        canTouch = true;
    }

    IEnumerator BlinkText()
    {
        for (int i = 0; i < 3; i++)
        {
            inputText.enabled = false;
            yield return new WaitForSeconds(0.15f);
            inputText.enabled = true;
            yield return new WaitForSeconds(0.15f);
        }

        currentNumber = "";
        inputText.text = "";
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignUIObjects();
    }

    void AssignUIObjects()
    {
        var callUIManagerObj = GameObject.Find("Calling_Image");
        if (callUIManagerObj != null)
        {
            callUIManager = callUIManagerObj.GetComponent<CallingUIManager>();

        }
    }
}
