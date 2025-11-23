using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject gameOverPanel;

    [Header("UI Dependencies")]
    public GameObject taskBoardUI; // From your old PauseManager

    [Header("Required References")]
    public GameObject player;

    // Private script references
    private PlayerMovement playerMovement;
    private CameraControl cameraControl;
    private CameraPivot cameraPivot;

    private bool isPaused = false;

    void Start()
    {
        // Get script components from the player and camera
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            // Make sure your camera scripts are named correctly
            if (Camera.main != null)
            {
                cameraControl = Camera.main.GetComponent<CameraControl>();
                if (Camera.main.transform.parent != null)
                {
                    cameraPivot = Camera.main.transform.parent.GetComponent<CameraPivot>();
                }
            }
        }

        // Ensure all UI panels are hidden at the start
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Start with the game running
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        // Check for the Escape key to pause/resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Don't pause if the game over screen is active
            if (gameOverPanel != null && gameOverPanel.activeInHierarchy)
            {
                return;
            }

            // Don't pause if the task board is open
            if (taskBoardUI != null && taskBoardUI.activeInHierarchy)
            {
                return;
            }

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // --- PAUSE & RESUME LOGIC ---

    private void PauseGame()
    {
        isPaused = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        SetPlayerAndCameraEnabled(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        SetPlayerAndCameraEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- GAME OVER LOGIC ---

    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
        SetPlayerAndCameraEnabled(false); // Also disable player on game over

        if (AudioManager.instance != null) AudioManager.instance.PlaySound("GameOver");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (AudioManager.instance != null) AudioManager.instance.SetWalkingState(false);
    }

    // --- SHARED BUTTON FUNCTIONS ---

    public void OnResumeButtonPressed()
    {
        PlayButtonClickSound();
        ResumeGame();
    }

    public void RestartLevel()
    {
        StartCoroutine(LoadSceneWithDelay(SceneManager.GetActiveScene().name));
    }

    public void GoToMainMenu()
    {
        StartCoroutine(LoadSceneWithDelay("Menu")); // Assumes your main menu scene is named "Menu"
    }

    // --- UTILITY FUNCTIONS ---

    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        PlayButtonClickSound();
        if (AudioManager.instance != null)
        {
            // Wait for the duration of the clip as told by the AudioManager
            yield return new WaitForSecondsRealtime(AudioManager.instance.GetSoundDuration("ButtonClick"));
        }

        Time.timeScale = 1f; // IMPORTANT: Always reset time scale before loading a new scene
        KeyPickup.playerHasKey = false;
        if (AudioManager.instance != null) AudioManager.instance.SetWalkingState(false);

        GameManager.isSceneTransitioning = true; // Set global flag for doors
        SceneManager.LoadScene(sceneName);
    }

    private void SetPlayerAndCameraEnabled(bool isEnabled)
    {
        if (playerMovement != null) playerMovement.enabled = isEnabled;
        if (cameraControl != null) cameraControl.enabled = isEnabled;
        if (cameraPivot != null) cameraPivot.enabled = isEnabled;
    }

    public void PlayButtonClickSound()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySound("ButtonClick");
    }
}