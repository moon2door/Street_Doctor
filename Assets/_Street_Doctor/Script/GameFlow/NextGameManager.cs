using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextGameManager : MonoBehaviour
{
    public GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // OVRCameraRig에 "Player" 태그 붙여야 함
        {
            gameManager.SetTriggeredObject(gameObject.name);
        }
    }
}
