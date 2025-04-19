using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tothelastscene : MonoBehaviour
{
    public Image[] images;           // 보여줄 이미지들
    private int currentIndex = 0;    // 현재 보여주고 있는 이미지 인덱스
    public bool allImagesShown = false;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // 시작 시 모든 이미지 꺼두기
        if (images != null)
        {
            foreach (var img in images)
            {
                if (img != null)
                    img.gameObject.SetActive(false);
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 병합된 씬이 현재 활성화된 씬일 때만 실행
        if (scene.name == "_Webtoon_Scene_01")
        {
            AssignUIObjects();
        }
    }

    void AssignUIObjects()
    {
        images = new Image[4];

        for (int i = 0; i < images.Length; i++)
        {
            string imageName = $"Cut_{i + 1}";
            GameObject imageObj = GameObject.Find(imageName);
            if (imageObj != null)
            {
                images[i] = imageObj.GetComponent<Image>();
                images[i].gameObject.SetActive(false); // 시작 시 꺼두기
            }
            else
            {
                Debug.LogWarning($"{imageName} 오브젝트를 찾을 수 없습니다.");
            }
        }

        currentIndex = 0;
        allImagesShown = false;
    }

    public void ShowNextImage()
    {
        if (currentIndex < images.Length && images[currentIndex] != null)
        {
            images[currentIndex].gameObject.SetActive(true);
            currentIndex++;

            if (currentIndex >= images.Length)
            {
                allImagesShown = true;
            }
        }
    }
}
