using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public static string nextSceneName = "3_Cut";

    private void Awake()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj != this.gameObject && obj.name == this.gameObject.name)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
