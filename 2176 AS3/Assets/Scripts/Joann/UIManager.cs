using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // an enum is used to create a simple, readable dropdown for the key's state in other scripts
    public enum KeyState { Uncollected, Collected }

    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject gameOverPanel;
    public GameObject hudPanel;
    public GameObject victoryPanel;
    public GameObject taskBoardPanel;

    [Header("HUD Elements")]
    public Image[] hearts; // an array to hold all the heart images for the health display
    public Slider escapeProgressBar;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Image keyImage;
    public Sprite uncollectedKeySprite;
    public Sprite collectedKeySprite;

    [Header("Timer Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI warningText;
    private LightInteraction currentTimedLight; // reference to the light that started the timer

    // internal variables to manage the light timer and cooldown functionality
    private int lastBeepSecond;
    private float timerValue = 0f;
    private bool isTimerActive = false;
    private bool isGlobalCooldownActive = false;

    // public properties that other scripts can read without being able to change the values
    public bool IsLightTimerActive => isTimerActive;
    public bool IsGlobalLightCooldownActive => isGlobalCooldownActive;

    [Header("Required References")]
    public GameObject player;

    // private script references are cached in start for better performance than using getcomponent repeatedly
    private PlayerMovement playerMovement;
    private CameraControl cameraControl;
    private CameraPivot cameraPivot;

    // internal state flags for managing ui states
    private bool isPaused = false;
    private bool isTaskBoardOpen = false;

    void Start()
    {
        // get and store references to player and camera control scripts at the start of the game
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            if (Camera.main != null)
            {
                cameraControl = Camera.main.GetComponent<CameraControl>();
                if (Camera.main.transform.parent != null)
                {
                    cameraPivot = Camera.main.transform.parent.GetComponent<CameraPivot>();
                }
            }
        }

        // set the initial visibility for all ui panels to ensure a clean start
        if (hudPanel != null) hudPanel.SetActive(true);
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (warningText != null) warningText.gameObject.SetActive(false);
        if (taskBoardPanel != null) taskBoardPanel.SetActive(false);

        UpdateKeyUI(KeyState.Uncollected);

        // ensure the game is not paused when the scene loads
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        // listen for the escape key every frame to handle pausing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // gives closing the task board priority over opening the pause menu
            if (isTaskBoardOpen)
            {
                ToggleTaskBoard();
                return; // stop further input processing for this frame
            }

            // prevents the pause menu from opening on top of the game over or victory screens
            if (gameOverPanel != null && gameOverPanel.activeInHierarchy) return;
            if (victoryPanel != null && victoryPanel.activeInHierarchy) return;

            // if no other menus are open, toggle the pause state
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // if a light timer is active, run its countdown logic
        if (isTimerActive)
        {
            HandleTimerCountdown();
        }
    }

    // a single function to handle all logic for showing/hiding the task board
    public void ToggleTaskBoard()
    {
        isTaskBoardOpen = !isTaskBoardOpen;

        taskBoardPanel.SetActive(isTaskBoardOpen);
        hudPanel.SetActive(!isTaskBoardOpen);

        // lock or unlock the cursor based on whether the menu is open
        Cursor.lockState = isTaskBoardOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isTaskBoardOpen;

        // disable player movement and camera controls while the board is open
        SetPlayerAndCameraEnabled(!isTaskBoardOpen);

        // pause the game while the task board is open
        if (isTaskBoardOpen)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    // updates the key sprite in the hud based on its collected state
    public void UpdateKeyUI(KeyState state)
    {
        if (keyImage == null || uncollectedKeySprite == null || collectedKeySprite == null)
        {
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

    // public method for other scripts to display a temporary warning message on screen
    public void ShowWarningText(string message, float duration)
    {
        if (warningText != null)
        {
            warningText.text = message;
            StartCoroutine(WarningTextRoutine(duration));
        }
    }

    // coroutine to handle hiding the warning text after a set duration
    private IEnumerator WarningTextRoutine(float duration)
    {
        warningText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        warningText.gameObject.SetActive(false);
    }

    // handles all the logic needed to properly pause the game
    private void PauseGame()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // stop all time-based movement and physics
        SetPlayerAndCameraEnabled(false); // disable player controls
        Cursor.lockState = CursorLockMode.None; // unlock the cursor
        Cursor.visible = true; // show the cursor
    }

    // handles all the logic needed to properly resume the game
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        SetPlayerAndCameraEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // handles all the logic for when the player dies
    public void ShowGameOverScreen()
    {
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        SetPlayerAndCameraEnabled(false);
        if (AudioManager.instance != null) AudioManager.instance.PlaySound("GameOver");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (AudioManager.instance != null) AudioManager.instance.SetWalkingState(false);
    }

    // updates the health display by changing heart sprites
    public void UpdateHealthUI(int currentHealth)
    {
        // loop through the hearts array
        for (int i = 0; i < hearts.Length; i++)
        {
            // if the index is less than the current health, show a full heart
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            // otherwise, show an empty heart
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    // updates the progress bar slider based on tasks completed
    public void UpdateProgressBar(int completed, int total)
    {
        if (escapeProgressBar != null)
        {
            // the slider value is a float between 0 and 1, so we divide the integer values
            escapeProgressBar.value = (float)completed / total;
        }
    }

    // this is called by a lightinteraction script to start a timed event
    public void StartLightTimer(float duration, LightInteraction lightToTrack)
    {
        timerValue = duration;
        currentTimedLight = lightToTrack;
        isTimerActive = true;
        timerText.gameObject.SetActive(true);
        lastBeepSecond = Mathf.CeilToInt(duration) + 1;
    }

    // this function runs every frame while a light timer is active
    private void HandleTimerCountdown()
    {
        timerValue -= Time.deltaTime;
        int currentSecond = Mathf.CeilToInt(timerValue);

        // beep logic here
        if (timerValue <= 3f && currentSecond < lastBeepSecond && currentSecond > 0) { }

        timerText.text = $"Light Time Remaining: {Mathf.Ceil(timerValue)}s";

        // when the timer runs out
        if (timerValue <= 0)
        {
            isTimerActive = false;
            if (currentTimedLight != null)
            {
                float cooldown = currentTimedLight.cooldownDuration;
                currentTimedLight.ResetLight(); // tell the light to reset itself
                StartCoroutine(GlobalCooldownRoutine(cooldown)); // start the cooldown timer
            }
        }
    }

    // this coroutine manages the global cooldown after a timed light deactivates
    private IEnumerator GlobalCooldownRoutine(float duration)
    {
        isGlobalCooldownActive = true;
        ShowCooldownUI();
        float cd = duration;
        while (cd > 0)
        {
            cd -= Time.deltaTime;
            UpdateCooldownUI(cd);
            yield return null;
        }
        HideCooldownUI();
        isGlobalCooldownActive = false;
    }

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

    // handles all logic for the victory screen
    public void ShowVictoryScreen()
    {
        hudPanel.SetActive(false);
        pauseMenuUI.SetActive(false);
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(true);

        Time.timeScale = 0f;
        if (AudioManager.instance != null) AudioManager.instance.PlaySound("Victory");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // public functions for ui buttons to call
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

    // coroutine to load a new scene, with a delay to allow button click sounds to play
    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        PlayButtonClickSound();
        if (AudioManager.instance != null)
        {
            // use waitforsecondsrealtime so the delay works even when the game is paused (timescale = 0)
            yield return new WaitForSecondsRealtime(AudioManager.instance.GetSoundDuration("ButtonClick"));
        }
        Time.timeScale = 1f; // always reset time scale before loading a new scene
        KeyPickup.playerHasKey = false;
        if (AudioManager.instance != null) AudioManager.instance.SetWalkingState(false);
        GameManager.isSceneTransitioning = true; // set global flag
        SceneManager.LoadScene(sceneName);
    }

    // a helper function to enable or disable all player control scripts at once
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