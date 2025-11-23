using UnityEngine;
using System.Collections.Generic; // Required for using List and HashSet

public class GameManager : MonoBehaviour
{
    public static bool isSceneTransitioning = false;

    [Header("Game Progress Settings")]
    [Tooltip("The total number of unique tasks the player must complete to escape.")]
    public int totalTasksToEscape = 3;

    [Header("Contributing Task Tags")]
    [Tooltip("Add the string tags of the interaction types that should count towards progress.")]
    public List<string> contributingTaskTags = new List<string>();

    [Header("Manager References")]
    public UIManager uiManager;

    private int tasksCompleted = 0;
    private HashSet<GameObject> completedTaskObjects = new HashSet<GameObject>();
    public bool AreAllTasksComplete { get; private set; } = false;

    void Awake()
    {
        isSceneTransitioning = false;
        tasksCompleted = 0;
        completedTaskObjects.Clear();
        AreAllTasksComplete = false; // reset on restart
    }

    // This is the public function that ALL interaction scripts will call.
    public void TryCompleteTask(GameObject taskObject)
    {
        // Check 1: Does this object's tag exist in our list of contributing tags?
        // If the list does not contain the object's tag, we ignore it.
        if (!contributingTaskTags.Contains(taskObject.tag))
        {
            return; // This interaction type is not part of the main quest.
        }

        // Check 2: Has this specific object already been completed?
        // This prevents the same door or board from being counted multiple times.
        if (completedTaskObjects.Add(taskObject))
        {
            // If it's a valid type AND a new object, increment the progress.
            tasksCompleted++;
            Debug.Log($"Task '{taskObject.name}' of type '{taskObject.tag}' Completed! Progress: {tasksCompleted}/{totalTasksToEscape}");

            if (uiManager != null)
            {
                uiManager.UpdateProgressBar(tasksCompleted, totalTasksToEscape);
            }

            if (tasksCompleted >= totalTasksToEscape)
            {
                Debug.Log("YOU WIN! All tasks completed.");
            }
            if (tasksCompleted >= totalTasksToEscape)
            {
                Debug.Log("YOU WIN! All tasks completed.");
                AreAllTasksComplete = true; // set flag to true
            }
        }
    }

    void OnApplicationQuit()
    {
        isSceneTransitioning = true;
    }
}