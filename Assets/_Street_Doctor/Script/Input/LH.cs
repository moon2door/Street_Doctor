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
    

    void Start()
    {
        myLR = GetComponent<LineRenderer>();
        left_hand = GameObject.Find("LeftHandAnchor");

        transform.position = left_hand.transform.position;
        transform.eulerAngles = left_hand.transform.eulerAngles;
        transform.parent = left_hand.transform;
               
    }

    void Update()
    {
        HandleMovement();
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
}

   