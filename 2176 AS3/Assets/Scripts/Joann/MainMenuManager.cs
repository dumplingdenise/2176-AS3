using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for using Coroutines

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Management")]
    [Tooltip("The name of the scene you want to load when the Play button is clicked.")]
    public string sceneToLoad = "Jo";

    [Header("Audio Settings")]
    public AudioClip buttonClickSound;
    private AudioSource audioSource;

    void Start()
    {
        // Get or add the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // This is called by the "Play" button's OnClick event
    public void PlayGame()
    {
        // Start the coroutine to handle the scene transition with a delay
        StartCoroutine(LoadSceneWithDelay(sceneToLoad));
    }

    // This is called by the "Quit" button's OnClick event
    public void QuitGame()
    {
        // Start the coroutine to handle quitting with a delay
        StartCoroutine(QuitWithDelay());
    }

    // Coroutine to play sound, wait, and then load the scene
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        // 1. Play the button click sound
        PlayButtonClickSound();

        // 2. Wait for the length of the audio clip before proceeding
        if (buttonClickSound != null)
        {
            yield return new WaitForSeconds(buttonClickSound.length);
        }

        // 3. Check if a scene name has been provided
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene To Load is not set in the MainMenuManager script! Please set it in the Inspector.");
            yield break; // Stop the coroutine
        }

        // 4. Now, load the scene
        SceneManager.LoadScene(sceneName);
    }

    // Coroutine to play sound, wait, and then quit the application
    private IEnumerator QuitWithDelay()
    {
        // 1. Play the button click sound
        PlayButtonClickSound();

        // 2. Wait for the length of the audio clip
        if (buttonClickSound != null)
        {
            yield return new WaitForSeconds(buttonClickSound.length);
        }

        // 3. Now, quit the application
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    // This function is now only responsible for playing the sound itself
    public void PlayButtonClickSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}