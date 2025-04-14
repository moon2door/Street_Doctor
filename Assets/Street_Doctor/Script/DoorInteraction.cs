using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public bool isOpen = false;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private bool isMoving = false;
    void Start()
    {
        closedRotation = transform.rotation;
        openedRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }
    public void OnInteract()
    {
        Debug.Log("OnInteract »£√‚µ ");
        if (!isMoving)
            StartCoroutine(RotateDoor());
    }
    IEnumerator RotateDoor()
    {
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