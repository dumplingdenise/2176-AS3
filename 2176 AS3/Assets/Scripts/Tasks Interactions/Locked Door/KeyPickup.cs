using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public static bool playerHasKey = false;
    public GameObject interactionUI;  // "Press E to pick up key"
    public GameObject keyUI;          // UI icon/text for showing key in HUD

    private bool canPickUp = false;

    void Start()
    {
        if (keyUI != null)
            keyUI.SetActive(false); // Hide key UI at start
    }

    void Update()
    {
        if (canPickUp && Input.GetKeyDown(KeyCode.E))
        {
            playerHasKey = true;
            interactionUI.SetActive(false);

            if (keyUI != null)
                keyUI.SetActive(true); // Show key UI on pickup

            Destroy(gameObject);  // Remove key from scene
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = true;
            interactionUI.SetActive(true);
            Debug.Log("Player touched key");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canPickUp = false;
            interactionUI.SetActive(false);
        }
    }
}
