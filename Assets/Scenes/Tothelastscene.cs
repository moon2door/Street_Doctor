
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tothelastscene : MonoBehaviour
{
    public Image[] images;
    public float delay = 3f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var img in images)
        {
            img.gameObject.SetActive(false);
        }

        StartCoroutine(StartSequence());
        StartCoroutine(sceneupload(15f));

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator StartSequence()
    {

        for (int i = 0; i < images.Length; i++)
        {
            yield return new WaitForSeconds(delay);

            images[i].gameObject.SetActive(true);

        }

    }

    IEnumerator sceneupload(float delay_Time)
    {

        yield return new WaitForSeconds(delay_Time);
        SceneManager.LoadScene("D");


    }
}
