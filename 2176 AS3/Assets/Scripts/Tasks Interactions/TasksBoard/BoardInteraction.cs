using UnityEngine;

public class BoardInteraction : MonoBehaviour
{
    public GameObject taskUI;
    public GameObject interactionText; // NEW
    public float interactDistance = 3f;
    public Transform player;

    private bool isOpen = false;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        // Show interaction text when close enough
        if (distance <= interactDistance && !isOpen)
        {
            interactionText.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))        // Opens / Interacts with the Task Board by trigger 'E' key
            {
                ToggleTaskUI();
            }
        }
        else
        {
            interactionText.SetActive(false);
        }

        // Close UI
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))     // Closes the Task Board by trigger 'Esc' key
        {
            ToggleTaskUI();
        }
    }

    void ToggleTaskUI()
    {
        isOpen = !isOpen;
        taskUI.SetActive(isOpen);

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }
}


