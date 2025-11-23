using UnityEngine;

public class PushInteractionUI_Shumin : MonoBehaviour
{
    [Header("World-space UI prompt")]
    [Tooltip("UI object that says something like 'Press E to push box'.")]
    public GameObject interactionUI;

    private void Start()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);   // hide at start
        }
        else
        {
            Debug.LogWarning("PushInteractionUI_Shumin: interactionUI is not assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (interactionUI != null)
        {
            interactionUI.SetActive(true);    // show 'Press E' text
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (interactionUI != null)
        {
            interactionUI.SetActive(false);   // hide when leaving
        }
    }
}
