using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool isSceneTransitioning = false;

    // --- NEW PROGRESS TRACKING ---
    [Header("Game Progress")]
    public int totalTasksToEscape = 3; // Set this in the Inspector
    private int tasksCompleted = 0;

    // We need a reference to the UIManager to update the progress bar
    [Header("References")]
    public UIManager uiManager;

    void Awake()
    {
        isSceneTransitioning = false;
        tasksCompleted = 0; // Reset progress on scene start
    }

    // This public function will be called by other scripts (like LightInteraction)
    public void CompleteTask()
    {
        if (tasksCompleted >= totalTasksToEscape) return; // Don't increment if already won

        tasksCompleted++;
        Debug.Log("Task Completed! Progress: " + tasksCompleted + "/" + totalTasksToEscape);

        // Tell the UI to update the progress bar
        if (uiManager != null)
        {
            uiManager.UpdateProgressBar(tasksCompleted, totalTasksToEscape);
        }

        // Check for win condition
        if (tasksCompleted >= totalTasksToEscape)
        {
            Debug.Log("YOU WIN! All tasks completed.");
            // You can add logic here to show a "You Win" screen later
        }
    }

    void OnApplicationQuit()
    {
        isSceneTransitioning = true;
    }
}