using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))       // Using 'E' to trigger the interaction of picking up the object.
        {
            PickUpKey();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            InteractionUI.instance.ShowMessage("Press E to pick up key");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            InteractionUI.instance.ClearMessage();
        }
    }

    private void PickUpKey()
    {
        PlayerInventory.hasKey = true;
        InteractionUI.instance.ShowMessage("Key Collected!");
        gameObject.SetActive(false);            // Can deactivate the key immediately (visuals/collider off)
        Invoke(nameof(ClearText), 1.0f);        // Ask the UI system (which should be always active) to clear the message after 1 second
    }

    void ClearText()
    {
        InteractionUI.instance.ClearMessage();
    }
}

