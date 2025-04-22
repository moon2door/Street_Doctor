using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CallingUIManager : MonoBehaviour
{
    public GameObject callCanvas;
    public Text guideText;
    public GameObject phoneObject;

    public Image cprcountImage;

    public KeypadTouchInput keypadTouchInput;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ShowCallUI();
    }

    public void ShowCallUI()
    {
        callCanvas.SetActive(true);
        phoneObject.SetActive(true);
        cprcountImage.gameObject.SetActive(false);
        guideText.text = "왼손의 휴대폰으로 119에 신고하세요!";
    }

    public void ShowCompleteMessage()
    {
        guideText.text = "신고가 완료되었습니다! \n이제 환자를 구출하러 가요!";
        phoneObject.SetActive(false);
        callCanvas.SetActive(false);
        cprcountImage.gameObject.SetActive(true);
        StartCoroutine(DeleteMessage());    
    }

    public void ShowErrorMessage()
    {
        guideText.text = "그 번호는 응급번호가 아니에요 ㅠㅠ \n응급번호 : 119";
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignUIObjects();
    }

    void AssignUIObjects()
    {
        phoneObject = GameObject.Find("Phone");
        keypadTouchInput = GameObject.Find("FingerTip_R ")?.GetComponent<KeypadTouchInput>();   
    }

    IEnumerator DeleteMessage()
    {
        yield return new WaitForSeconds(3f);

        guideText.gameObject.SetActive(false);
    }
}
