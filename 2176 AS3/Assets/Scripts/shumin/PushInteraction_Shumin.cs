using UnityEngine;

public class PushInteraction_Shumin : MonoBehaviour
{
    [Header("Box to push")]
    public Rigidbody boxRb;

    private bool isPlayerNear = false;

    private bool hasUnlocked = false;
    public bool HasUnlocked => hasUnlocked;   // read-only from other scripts

    private void Start()
    {
        if (boxRb == null)
        {
            Debug.LogError("PushInteraction_Shumin: boxRb is not assigned!");
            return;
        }

        boxRb.isKinematic = true;   // start locked
        hasUnlocked = false;
    }

    private void Update()
    {
        if (!isPlayerNear || boxRb == null || hasUnlocked)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            boxRb.isKinematic = false;
            hasUnlocked = true;     // mark as permanently unlocked
            Debug.Log("PushInteraction: box unlocked.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = false;
    }
}
