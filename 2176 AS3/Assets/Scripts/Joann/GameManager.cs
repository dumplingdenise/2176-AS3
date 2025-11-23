using UnityEngine;

public class GameManager : MonoBehaviour
{
    // This 'static' variable can be accessed from any script in your game
    // using GameManager.isSceneTransitioning
    public static bool isSceneTransitioning = false;

    // This runs when the script instance is being loaded.
    void Awake()
    {
        // When a new scene loads and this script awakens, we know
        // the transition is complete. So, we reset the flag.
        isSceneTransitioning = false;
    }

    // This function is called when the application quits or the editor stops.
    // This prevents sounds from playing when you stop the game in the editor.
    void OnApplicationQuit()
    {
        isSceneTransitioning = true;
    }
}