using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class KeypadTouchInput : MonoBehaviour
{
    public Text inputText;                 // 전화번호 출력용 텍스트
    public CallingUIManager callUIManager;   // 신고 UI 제어
    public GameManager gameManager;

    private string currentNumber = "";

    private bool canTouch = true;         // 터치 딜레이 컨트롤

    public AudioClip effectSound;           // 한 번 재생할 사운드 클립
    public AudioClip effectSound1;           // 한 번 재생할 사운드 클립
    private AudioSource audioSource;        // AudioSource 컴포넌트

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (callUIManager == null)
        {
            AssignUIObjects();
        }
        else
        {
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canTouch) return;

        string tag = other.tag;

        if (tag.StartsWith("Key"))
        {
            audioSource.PlayOneShot(effectSound);

            string key = tag.Replace("Key", ""); // Key1 → 1
            currentNumber += key;
            inputText.text = currentNumber;
            Debug.Log($"🔢 입력된 숫자: {key}");
            StartCoroutine(TouchDelay());
        }
        else if (tag == "CallButton")
        {
            audioSource.PlayOneShot(effectSound);

            Debug.LogError("콜버튼 눌림!");

            StartCoroutine(TouchDelay());

            if (currentNumber == "119")
            {
                Debug.Log("✅ 119 신고 완료!");
                audioSource.PlayOneShot(effectSound1);
                callUIManager.ShowCompleteMessage();
                gameManager.StartGamePhase();

            }
            else
            {
                Debug.Log("❌ 잘못된 번호 입력");
                callUIManager.ShowErrorMessage();
                StartCoroutine(BlinkText());
            }
        }
        else
        {
            Debug.LogError("아무것도 입력안됨!");
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
        Debug.Log("실행중");

        var callUIManagerObj = GameObject.Find("Calling_Image");
        if (callUIManagerObj != null)
        {
            callUIManager = callUIManagerObj.GetComponent<CallingUIManager>();

        }
    }
}
