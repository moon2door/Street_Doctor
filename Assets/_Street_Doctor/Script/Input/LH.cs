using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LH : MonoBehaviour
{
    LineRenderer myLR;
    GameObject left_hand;
    Ray ray;
    RaycastHit hit;

    public float moveSpeed = 2.0f;
    public Transform playerRoot;
    private GameObject grabbedObject = null;
    public GameManager gameManager;

    void Start()
    {
        myLR = GetComponent<LineRenderer>();
        left_hand = GameObject.Find("LeftHandAnchor");

        transform.position = left_hand.transform.position;
        transform.eulerAngles = left_hand.transform.eulerAngles;
        transform.parent = left_hand.transform;

        GameObject gmObj = GameObject.Find("GameManager");
        if (gmObj != null)
            gameManager = gmObj.GetComponent<GameManager>();
        else
            Debug.LogWarning("GameManager 오브젝트를 찾지 못했습니다. LH에서");


        StartCoroutine(AssignUIObjects001());
    }

    void Update()
    {
        HandleMovement();
        HandleInteractionRay();
        HandleGrabRelease();
    }

    void HandleMovement()
    {
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector3 moveDir = left_hand.transform.right * input.x + left_hand.transform.forward * input.y;
        moveDir.y = 0f;

        if (playerRoot != null)
        {
            Rigidbody rb = playerRoot.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (input.sqrMagnitude > 0.01f)
                {
                    rb.MovePosition(rb.position + moveDir * moveSpeed * Time.deltaTime);
                }
                else if (!rb.isKinematic)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }
    }

    void HandleInteractionRay()
    {
        ray.origin = left_hand.transform.position;
        ray.direction = left_hand.transform.forward;
        myLR.SetPosition(0, ray.origin);
        myLR.SetPosition(1, ray.origin + ray.direction * 3);

        if (Physics.Raycast(ray, out hit, 3f))
        {
            myLR.startColor = Color.green;
            myLR.endColor = Color.green;
            myLR.SetPosition(1, hit.point);

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
            {
                DoorInteraction door = hit.collider.gameObject.GetComponentInParent<DoorInteraction>();
                door?.OnInteract();
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) && hit.collider.GetComponent<GrabObject>() != null)
            {
                grabbedObject = hit.collider.gameObject;
                grabbedObject.transform.position = left_hand.transform.position + ray.direction * 0.1f;
                grabbedObject.transform.parent = left_hand.transform;
                grabbedObject.transform.eulerAngles = Vector3.zero;
            }

            if (grabbedObject != null && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
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

    void HandleGrabRelease()
    {
        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch) && grabbedObject != null)
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

    IEnumerator ShortVibration(float duration)
    {
        OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.LTouch);
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }

    IEnumerator AssignUIObjects001()
    {
        yield return new WaitForSeconds(0.5f);

        playerRoot = GameObject.Find("PlayerRoot(Clone)")?.GetComponent<Transform>();
    }
}