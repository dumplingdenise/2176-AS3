using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static bool isSceneTransitioning = false;

    [Header("Game Progress Settings")]
    [Tooltip("The total number of unique tasks the player must complete to escape.")]
    public int totalTasksToEscape = 3;

    [Header("Manager References")]
    public UIManager uiManager;

    private const string interactionTaskTag = "InteractionTask";

    private int tasksCompleted = 0;
    private HashSet<GameObject> completedTaskObjects = new HashSet<GameObject>();
    public bool AreAllTasksComplete { get; private set; } = false;

    void Awake()
    {
        isSceneTransitioning = false;
        tasksCompleted = 0;
        completedTaskObjects.Clear();
        AreAllTasksComplete = false;
    }

    public void TryCompleteTask(GameObject taskObject)
    {
        if (taskObject.tag != interactionTaskTag)
        {
            return;
        }

        if (completedTaskObjects.Add(taskObject))
        {
            tasksCompleted++;
            Debug.Log($"Task '{taskObject.name}' Completed! Progress: {tasksCompleted}/{totalTasksToEscape}");

            if (uiManager != null)
            {
                uiManager.UpdateProgressBar(tasksCompleted, totalTasksToEscape);
            }

            if (tasksCompleted >= totalTasksToEscape)
            {
                Debug.Log("YOU WIN! All tasks completed.");
                AreAllTasksComplete = true;
            }
        }
    }

    void OnApplicationQuit()
    {
        isSceneTransitioning = true;
    }
}