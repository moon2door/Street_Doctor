using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    public static string nextSceneName = "3_Cut";

    private void Awake()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj != this.gameObject && obj.name == this.gameObject.name)
            {
                Destroy(obj);
                Debug.Log($"ªË¡¶µ : {obj.name}");
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
