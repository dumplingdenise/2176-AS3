using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // a global flag that other scripts can check to prevent actions during scene loads, like playing sounds
    public static bool isSceneTransitioning = false;

    [Header("Game Progress Settings")]
    [Tooltip("The total number of unique tasks the player must complete to escape.")]
    public int totalTasksToEscape = 3;

    [Header("Manager References")]
    public UIManager uiManager;

    // using a const string for the tag prevents typos in the code
    private const string interactionTaskTag = "InteractionTask";

    // internal counters for tracking game progress
    private int tasksCompleted = 0;
    // a hashset is used here to efficiently store completed objects & prevent counting the same task twice
    private HashSet<GameObject> completedTaskObjects = new HashSet<GameObject>();
    // a public property that other scripts (like the final door) can read to check if the game is won
    public bool AreAllTasksComplete { get; private set; } = false;

    void Awake()
    {
        // reset all game progress variables at the start of the scene
        isSceneTransitioning = false;
        tasksCompleted = 0;
        completedTaskObjects.Clear();
        AreAllTasksComplete = false;
    }

    // public method that interactable objects call to register task completion
    public void TryCompleteTask(GameObject taskObject)
    {
        // first, check if the interacted object is actually a task object
        if (taskObject.tag != interactionTaskTag)
        {
            return;
        }

        // the .add method of a hashset returns true only if the item was not already in the set
        if (completedTaskObjects.Add(taskObject))
        {
            // if it's a new task, increment the counter
            tasksCompleted++;
            Debug.Log($"Task '{taskObject.name}' Completed! Progress: {tasksCompleted}/{totalTasksToEscape}");

            // update the visual progress bar in the ui
            if (uiManager != null)
            {
                uiManager.UpdateProgressBar(tasksCompleted, totalTasksToEscape);
            }

            // check if the number of completed tasks has reached the win condition
            if (tasksCompleted >= totalTasksToEscape)
            {
                Debug.Log("YOU WIN! All tasks completed.");
                AreAllTasksComplete = true; // set the public flag so other scripts know the game is won
            }
        }
    }

    // a unity callback function that runs just before the application closes
    void OnApplicationQuit()
    {
        isSceneTransitioning = true;
    }
}