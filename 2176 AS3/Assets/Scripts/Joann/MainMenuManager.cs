using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // required for using coroutines

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Management")]
    [Tooltip("The name of the scene you want to load when the Play button is clicked.")]
    public string sceneToLoad = "Jo";

    [Header("Audio Settings")]
    public AudioClip buttonClickSound;
    private AudioSource audioSource; // a private reference to the component that will play the sound

    void Start()
    {
        // get or add the audiosource component at runtime
        audioSource = GetComponent<AudioSource>();
        // this ensures the script won't fail if an audiosource isn't manually added in the editor
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // this public method is called by the "play" button's onclick event in the unity editor
    public void PlayGame()
    {
        // starts the coroutine to handle the scene transition with a delay for the sound
        StartCoroutine(LoadSceneWithDelay(sceneToLoad));
    }

    // this public method is called by the "quit" button's onclick event
    public void QuitGame()
    {
        // starts the coroutine to handle quitting the application with a delay
        StartCoroutine(QuitWithDelay());
    }

    // coroutine to play sound, wait, and then load the scene
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        // 1. play the button click sound
        PlayButtonClickSound();

        // 2. wait for the length of the audio clip before proceeding
        if (buttonClickSound != null)
        {
            // 'yield return' pauses the coroutine's execution for a set amount of time
            yield return new WaitForSeconds(buttonClickSound.length);
        }

        // 3. check if a scene name has been provided to prevent errors
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene To Load is not set in the MainMenuManager script! Please set it in the Inspector.");
            yield break; // stops the coroutine if the scene name is missing
        }

        // 4. now, load the specified scene
        SceneManager.LoadScene(sceneName);
    }

    // coroutine to play sound, wait, and then quit the application
    private IEnumerator QuitWithDelay()
    {
        // 1. play the button click sound
        PlayButtonClickSound();

        // 2. wait for the length of the audio clip
        if (buttonClickSound != null)
        {
            yield return new WaitForSeconds(buttonClickSound.length);
        }

        // 3. now, quit the application
        Debug.Log("Quitting Game..."); // this log helps confirm the button works in the editor
        Application.Quit();
    }

    // this function is now only responsible for playing the sound itself
    public void PlayButtonClickSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            // use playoneshot to allow the sound to play even if another sound is already playing
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}