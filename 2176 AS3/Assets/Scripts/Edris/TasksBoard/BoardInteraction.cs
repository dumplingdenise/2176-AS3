using UnityEngine;

public class BoardInteraction : MonoBehaviour
{
    public GameObject interactionText; 
    public float interactDistance = 3f;
    public Transform player;
    public GameManager gameManager;
    public UIManager uiManager;

    private bool taskCompleted = false;

    void Start()
    {
        if (uiManager == null)
        {
            Debug.LogError("BoardInteraction is missing a reference to the UIManager!", this.gameObject);
        }
        if (interactionText != null) interactionText.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            if (uiManager != null && !uiManager.taskBoardPanel.activeInHierarchy)
            {
                interactionText.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {

                if (uiManager != null)
                {
                    uiManager.ToggleTaskBoard();
                }

                if (!taskCompleted && gameManager != null)
                {
                    gameManager.TryCompleteTask(this.gameObject);
                    taskCompleted = true;
                }
            }
        }
        else
        {
            interactionText.SetActive(false);
        }
    }

}