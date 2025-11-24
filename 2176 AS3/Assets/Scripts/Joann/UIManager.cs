using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // enum to define key's state
    public enum KeyState { Uncollected, Collected }

    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject gameOverPanel;
    public GameObject hudPanel;
    public GameObject victoryPanel;
    public GameObject taskBoardPanel;

    [Header("HUD Elements")]
    public Image[] hearts; // An array to hold our heart images
    public Slider escapeProgressBar;
    public Sprite fullHeart;
    public Sprite emptyHeart; 
    public Image keyImage;
    public Sprite uncollectedKeySprite;
    public Sprite collectedKeySprite;

    [Header("Timer Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI warningText;
    private LightInteraction currentTimedLight;

    private int lastBeepSecond;
    private float timerValue = 0f;
    private bool isTimerActive = false;
    private bool isGlobalCooldownActive = false;

    public bool IsLightTimerActive => isTimerActive;
    public bool IsGlobalLightCooldownActive => isGlobalCooldownActive;

    [Header("Required References")]
    public GameObject player;

    // Private script references
    private PlayerMovement playerMovement;
    private CameraControl cameraControl;
    private CameraPivot cameraPivot;

    private bool isPaused = false;
    private bool isTaskBoardOpen = false;

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
        if (warningText != null) warningText.gameObject.SetActive(false);
        if (taskBoardPanel != null) taskBoardPanel.SetActive(false);

        UpdateKeyUI(KeyState.Uncollected);

        // Start with the game running
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        // Check for the Escape key to pause/resume OR close the task board
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // PRIORITY 1: If the task board is open, close it and do nothing else.
            if (isTaskBoardOpen)
            {
                ToggleTaskBoard(); // This will close the board.
                return; // Stop further input processing for this frame.
            }

            // PRIORITY 2: If other menus are open, do not open the pause menu.
            if (gameOverPanel != null && gameOverPanel.activeInHierarchy) return;
            if (victoryPanel != null && victoryPanel.activeInHierarchy) return;

            // PRIORITY 3: If nothing else is open, toggle the pause menu.
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

    public void ToggleTaskBoard()
    {
        isTaskBoardOpen = !isTaskBoardOpen;

        if (taskBoardPanel != null) taskBoardPanel.SetActive(isTaskBoardOpen);

        if (hudPanel != null) hudPanel.SetActive(!isTaskBoardOpen);

        Cursor.lockState = isTaskBoardOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isTaskBoardOpen;

        SetPlayerAndCameraEnabled(!isTaskBoardOpen);

        if (isTaskBoardOpen)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void UpdateKeyUI(KeyState state)
    {
        if (keyImage == null || uncollectedKeySprite == null || collectedKeySprite == null)
        {
            Debug.LogWarning("One of the key UI elements is not assigned in the UIManager!");
            return;
        }

        switch (state)
        {
            case KeyState.Uncollected:
                keyImage.sprite = uncollectedKeySprite;
                break;
            case KeyState.Collected:
                keyImage.sprite = collectedKeySprite;
                break;
        }
    }

    public void ShowWarningText(string message, float duration)
    {
        if (warningText != null)
        {
            warningText.text = message;
            StartCoroutine(WarningTextRoutine(duration));
        }
    }

    private IEnumerator WarningTextRoutine(float duration)
    {
        warningText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        warningText.gameObject.SetActive(false);
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

        lastBeepSecond = Mathf.CeilToInt(duration) + 1;
    }

    private void HandleTimerCountdown()
    {
        timerValue -= Time.deltaTime;
        int currentSecond = Mathf.CeilToInt(timerValue);

        if (timerValue <= 3f && currentSecond < lastBeepSecond && currentSecond > 0) { /* Beep logic is fine */ }

        if (timerText != null) { timerText.text = $"Light Time Remaining: {Mathf.Ceil(timerValue)}s"; }

        if (timerValue <= 0)
        {
            isTimerActive = false;
            // Note: We don't hide the timerText here, the cooldown will handle it.

            if (currentTimedLight != null)
            {
                // Get the cooldown duration from the specific light that just finished.
                float cooldown = currentTimedLight.cooldownDuration;
                // Reset the light object itself.
                currentTimedLight.ResetLight();
                // Start the UIManager's global cooldown routine.
                StartCoroutine(GlobalCooldownRoutine(cooldown));
            }

            Debug.Log("Light timer finished! Global cooldown started.");
        }
    }

    // --- NEW: This coroutine now lives in the UIManager ---
    private IEnumerator GlobalCooldownRoutine(float duration)
    {
        isGlobalCooldownActive = true;

        // Take control of the UI text to show the cooldown
        ShowCooldownUI();
        float cd = duration;

        while (cd > 0)
        {
            cd -= Time.deltaTime;
            UpdateCooldownUI(cd);
            yield return null;
        }

        // Cooldown finished
        HideCooldownUI();
        isGlobalCooldownActive = false;
    }

    // ---------- Cooldown Timer UI --------------
    public void ShowCooldownUI()
    {
        timerText.gameObject.SetActive(true);
    }

    public void UpdateCooldownUI(float timeLeft)
    {
        timerText.text = $"Light Toggle Cooldown: {Mathf.Ceil(timeLeft)}s";
    }

    public void HideCooldownUI()
    {
        timerText.gameObject.SetActive(false);
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
        KeyPickup.playerHasKey = false;
        UpdateKeyUI(KeyState.Uncollected);
        StartCoroutine(LoadSceneWithDelay(SceneManager.GetActiveScene().name));
    }

    public void GoToMainMenu()
    {
        KeyPickup.playerHasKey = false;
        UpdateKeyUI(KeyState.Uncollected);
        StartCoroutine(LoadSceneWithDelay("Menu"));
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