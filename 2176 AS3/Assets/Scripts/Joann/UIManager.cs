using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject gameOverPanel;
    public GameObject hudPanel;
    public GameObject victoryPanel;

    [Header("HUD Elements")]
    public Image[] hearts; // An array to hold our heart images
    public Slider escapeProgressBar;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Timer Elements")]
    public TextMeshProUGUI timerText;
    private bool isTimerActive = false;
    private float timerValue = 0f;
    private LightInteraction currentTimedLight;

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
        if (hudPanel != null) hudPanel.SetActive(true);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

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

        if (isTimerActive)
        {
            HandleTimerCountdown();
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
        if (hudPanel != null) hudPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
        SetPlayerAndCameraEnabled(false); // Also disable player on game over

        if (AudioManager.instance != null) AudioManager.instance.PlaySound("GameOver");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (AudioManager.instance != null) AudioManager.instance.SetWalkingState(false);
    }

    // --- HUD FUNCTIONS ---
    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    public void UpdateProgressBar(int completed, int total)
    {
        if (escapeProgressBar != null)
        {
            // The slider value goes from 0 to 1, so we divide
            escapeProgressBar.value = (float)completed / total;
        }
    }

    // --- TIMER FUNCTIONS ---
    public void StartLightTimer(float duration, LightInteraction lightToTrack)
    {
        timerValue = duration;
        currentTimedLight = lightToTrack;
        isTimerActive = true;
        if (timerText != null) timerText.gameObject.SetActive(true);
    }

    private void HandleTimerCountdown()
    {
        timerValue -= Time.deltaTime;
        if (timerText != null)
        {
            timerText.text = $"Time Remaining: {Mathf.Ceil(timerValue)}s";
        }

        if (timerValue <= 0)
        {
            isTimerActive = false;
            if (timerText != null) timerText.gameObject.SetActive(false);

            // Tell the light to turn off
            if (currentTimedLight != null)
            {
                currentTimedLight.ResetLight();
                StartCoroutine(StartCooldown(currentTimedLight));
            }

            Debug.Log("Light timer finished!");
        }
    }

    // ---------- Cooldown Timer --------------
    private IEnumerator StartCooldown(LightInteraction light)
    {
        float cd = light.cooldownDuration;

        // Show cooldown UI
        timerText.gameObject.SetActive(true);

        while (cd > 0)
        {
            cd -= Time.deltaTime;
            timerText.text = $"Cooldown: {Mathf.Ceil(cd)}s";
            yield return null;
        }

        // Cooldown finished, hide UI and allow light interaction again
        timerText.gameObject.SetActive(false);
        light.lightText.enabled = true; // show "Press E" again
    }

    // --- VICTORY FUNCTIONS ---
    public void ShowVictoryScreen()
    {
        // Hide all other UI
        if (hudPanel != null) hudPanel.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Show the victory panel
        if (victoryPanel != null) victoryPanel.SetActive(true);

        // Stop the game and play the sound
        Time.timeScale = 0f;
        if (AudioManager.instance != null) AudioManager.instance.PlaySound("Victory");

        // Unlock the cursor so the player can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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