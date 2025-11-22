using UnityEngine;

public class BoardInteraction : MonoBehaviour
{
    public GameObject taskUI;        // UI Panel
    public float interactDistance = 3f;
    public Transform player;

    private bool isOpen = false;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E))        // Trigger key 'E' to interact to see the Tasks on the Task UI Board.
        {
            ToggleTaskUI();
        }

        if (isOpen && Input.GetKeyDown(KeyCode.Escape))         // Trigger 'Esc' key to exit the Task UI Board.
        {
            ToggleTaskUI();
        }
    }

    void ToggleTaskUI()
    {
        isOpen = !isOpen;
        taskUI.SetActive(isOpen);

        // Lock player movement and cursor
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }
}

