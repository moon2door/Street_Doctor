using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RH : MonoBehaviour
{
    LineRenderer myLR;
    GameObject right_H;
    Ray ray;
    RaycastHit hit;

    public Transform playerRoot;
    public float rotationSpeed = 45f;

    public GaugeManager gaugeManager;
    public GameManager gameManager;
    public Tothelastscene tothelastscene;

    private GameObject grabbedObject = null;
    private bool doorOpened = false;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()
    {
        myLR = GetComponent<LineRenderer>();
        right_H = GameObject.Find("RightHandAnchor");
        transform.position = right_H.transform.position;
        transform.eulerAngles = right_H.transform.eulerAngles;
        transform.parent = right_H.transform;
    }

    void Update()
    {
        HandleInteractionRay();
        HandlePlayerRotation();
        HandleGrabRelease();
    }

    void HandleInteractionRay()
    {
        ray.origin = right_H.transform.position;
        ray.direction = right_H.transform.forward;
        myLR.SetPosition(0, ray.origin);
        myLR.SetPosition(1, ray.origin + ray.direction * 3);

        if (Physics.Raycast(ray, out hit, 3f))
        {
            myLR.startColor = Color.green;
            myLR.endColor = Color.green;
            myLR.SetPosition(1, hit.point);

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                DoorInteraction door = hit.collider.gameObject.GetComponentInParent<DoorInteraction>();
                if (door != null)
                {
                    door.OnInteract();

                    if (!doorOpened && door.isOpen)
                    {
                        doorOpened = true;
                        StartCoroutine(WaitAndChangeScene());
                    }
                }
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch) && hit.collider.GetComponent<GrabObject>() != null)
            {
                grabbedObject = hit.collider.gameObject;
                grabbedObject.transform.position = right_H.transform.position + ray.direction * 0.1f;
                grabbedObject.transform.parent = right_H.transform;
                grabbedObject.transform.eulerAngles = Vector3.zero;
            }

            if (grabbedObject != null && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                if (hit.collider.GetComponent<AttachableSpot>() is AttachableSpot spot && spot.snapTransform != null)
                {
                    grabbedObject.transform.position = spot.snapTransform.position;
                    grabbedObject.transform.rotation = spot.snapTransform.rotation;
                    grabbedObject.transform.parent = spot.snapTransform;
                    StartCoroutine(ShortVibration(0.1f));
                    grabbedObject = null;
                }
            }
        }
        else
        {
            myLR.startColor = Color.red;
            myLR.endColor = Color.red;
        }
    }

    void HandlePlayerRotation()
    {
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (Mathf.Abs(input.x) > 0.5f)
        {
            float rotationAmount = input.x * rotationSpeed * Time.deltaTime;
            playerRoot.Rotate(Vector3.up, rotationAmount);
        }
    }

    void HandleGrabRelease()
    {
        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch) && grabbedObject != null)
        {
            grabbedObject.transform.parent = null;
            GameObject location = GameObject.Find(grabbedObject.name + "_");
            if (location != null)
            {
                grabbedObject.transform.eulerAngles = Vector3.zero;
                grabbedObject.transform.parent = location.transform;
                grabbedObject.transform.localPosition = Vector3.zero;
            }
            grabbedObject = null;
        }
    }

    IEnumerator WaitAndChangeScene()
    {
        yield return new WaitForSeconds(2f);
        DontDestroy.nextSceneName = "_Webtoon_Scene_01";
        yield return StartCoroutine(gameManager.FadeToBlack());
        SceneManager.LoadScene("__Loading_Scene");
    }

    IEnumerator ShortVibration(float duration)
    {
        OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.RTouch);
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "_InGame_Scene_01") AssignUIObjects();
    }

    void AssignUIObjects()
    {
        var gaugeManagerObj = GameObject.Find("GaugeManager");
        if (gaugeManagerObj != null)
        {
            gaugeManager = gaugeManagerObj.GetComponent<GaugeManager>();
            if (gaugeManager.gaugeSlider == null)
            {
                var sliderObj = GameObject.Find("gaugeSlider");
                if (sliderObj != null)
                    gaugeManager.gaugeSlider = sliderObj.GetComponent<Slider>();
            }
        }
    }
}