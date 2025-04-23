using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour //Door_parent에 들어있는 스크립트. 힌지 역할을 한다.
{
    public bool isOpen = false;
    public float openAngle = 90f; //문 열림 각도
    public float openSpeed = 2f;  //문 열림 속도

    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private bool isMoving = false;

    public AudioClip effectSound;           // 한 번 재생할 사운드 클립
    private AudioSource audioSource;        // AudioSource 컴포넌트

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        closedRotation = transform.rotation;
        openedRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }
    public void OnInteract()
    {
        Debug.Log("OnInteract 호출됨");
        if (!isMoving)
            StartCoroutine(RotateDoor());
    }
    IEnumerator RotateDoor()
    {
        audioSource.PlayOneShot(effectSound);

        isMoving = true;
        Quaternion targetRotation = isOpen ? closedRotation : openedRotation;
        Quaternion startRotation = transform.rotation;

        float time = 0f;
        while (time < 1f)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time);
            time += Time.deltaTime * openSpeed;
            yield return null;
        }
        transform.rotation = targetRotation;
        isOpen = !isOpen;
        isMoving = false;
    }
}