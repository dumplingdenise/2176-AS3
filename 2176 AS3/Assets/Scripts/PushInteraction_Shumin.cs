using UnityEngine;

public class PushInteraction_Shumin : MonoBehaviour
{
    [Header("Box to push")]
    [Tooltip("Rigidbody on the pushable box (the one with the solid collider).")]
    public Rigidbody boxRb;               // drag the box Rigidbody here

    [Header("Settings")]
    [Tooltip("If true, the box starts locked (not pushable) until E is pressed.")]
    public bool startsLocked = true;

    private bool playerInRange = false;
    private bool isPushable = false;

    private void Start()
    {
        if (boxRb == null)
        {
            Debug.LogError("PushInteraction_Shumin: boxRb is not assigned!");
            return;
        }

        if (startsLocked)
        {
            boxRb.isKinematic = true;     // not pushable at start
            isPushable = false;
        }
        else
        {
            boxRb.isKinematic = false;    // immediately pushable
            isPushable = true;
        }
    }

    private void Update()
    {
        if (!playerInRange || boxRb == null)
            return;

        // Press E to toggle pushable / not pushable while near
        if (Input.GetKeyDown(KeyCode.E))
        {
            isPushable = !isPushable;
            boxRb.isKinematic = !isPushable;

            Debug.Log("PushInteraction: box pushable = " + isPushable);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        Debug.Log("PushInteraction: player entered push area.");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;

        // When player leaves, always lock the box again
        if (boxRb != null)
        {
            isPushable = false;
            boxRb.isKinematic = true;
            boxRb.linearVelocity = Vector3.zero;        // stop sliding
            boxRb.angularVelocity = Vector3.zero; // stop spinning
        }

        Debug.Log("PushInteraction: player left, box locked again.");
    }
}
