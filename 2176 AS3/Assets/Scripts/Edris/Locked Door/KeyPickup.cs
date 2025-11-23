using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public static bool playerHasKey = false;
    public GameObject interactionUI;  // "Press E to pick up key"
    public GameObject keyUI;          // UI icon/text for showing key in HUD

    private bool canPickUp = false;

    public GameManager gameManager;

    void Start()
    {
        if (keyUI != null)
            keyUI.SetActive(false); // Hide key UI at start
        else
            Debug.LogWarning("KeyPickup: keyUI is not assigned!");

        if (interactionUI == null)
            Debug.LogWarning("KeyPickup: interactionUI is not assigned!");
    }

    void Update()
    {
        if (!canPickUp) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // AUDIO (safe check)
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySound("KeyPickup");
            else
                Debug.LogWarning("KeyPickup: AudioManager instance is missing!");

            playerHasKey = true;

            if (interactionUI != null)
                interactionUI.SetActive(false);

            if (keyUI != null)
                keyUI.SetActive(true);

            // INTERACTION TRACKING
            if (gameManager != null) gameManager.TryCompleteTask(this.gameObject);

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = true;

            if (interactionUI != null)
                interactionUI.SetActive(true);
            else
                Debug.LogWarning("KeyPickup: interactionUI is not assigned!");

            Debug.Log("Player touched key");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = false;

            if (interactionUI != null)
                interactionUI.SetActive(false);
        }
    }
}

