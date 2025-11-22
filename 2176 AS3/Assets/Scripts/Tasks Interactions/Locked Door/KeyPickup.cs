using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public static bool playerHasKey = false;
    public GameObject interactionUI; // "Press E to pick up key"

    private bool canPickUp = false;

    void Update()
    {
        if (canPickUp && Input.GetKeyDown(KeyCode.E))
        {
            playerHasKey = true;
            interactionUI.SetActive(false);
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

