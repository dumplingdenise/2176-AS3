using UnityEngine;

public class PushInteraction_Shumin : MonoBehaviour
{
    // box to unlock
    public Rigidbody rb;
    private bool isplayerNear = false;

    public GameObject interactionUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.isKinematic = true;

        // UI starts hidden
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isplayerNear) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // unlock box so it can be pushed
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            // hide UI once they start pushing
            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isplayerNear = true;

        // show UI when player is in the zone
        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isplayerNear = false;

        // lock the box again when player leaves
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // hide UI when they walk away
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
}
