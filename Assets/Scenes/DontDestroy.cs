using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // 자기 자신은 제외하고, 같은 이름의 오브젝트가 있으면 파괴
            if (obj != this.gameObject && obj.name == this.gameObject.name)
            {
                Debug.Log(this.gameObject.name + " 파괴됨");
                Destroy(this.gameObject);
                return;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
