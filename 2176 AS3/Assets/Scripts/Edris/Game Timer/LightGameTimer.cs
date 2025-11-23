using UnityEngine;
using TMPro;

public class LightTimer : MonoBehaviour
{
    [Header("References")]
    public LightInteraction lightInteraction;   // Reference to your LightInteraction script
    public TextMeshProUGUI uiTimerText;         // UI text for the countdown

    [Header("Timer Settings")]
    public float duration = 10f;                // Countdown duration in seconds

    private float timer = 0f;
    private bool timerActive = false;

    void Start()
    {
        // Make sure the timer text is hidden at the start
        if (uiTimerText != null)
        {
            uiTimerText.enabled = false;
        }
    }

    void Update()
    {
        if (lightInteraction == null) return;

        // Start timer when light is switched on
        if (!timerActive && lightInteraction.light.enabled && lightInteraction.CanInteract == false)
        {
            timerActive = true;
            timer = duration;

            // Show the countdown text
            if (uiTimerText != null)
            {
                uiTimerText.enabled = true;
            }
        }

        // Countdown logic
        if (timerActive)
        {
            timer -= Time.deltaTime;

            // Update the timer display
            if (uiTimerText != null)
            {
                uiTimerText.text = $"Time left: {Mathf.Ceil(timer)}s";
            }

            // Timer finished
            if (timer <= 0f)
            {
                timerActive = false;
                timer = 0f;
                OnTimerFinished();
            }
        }
    }

    private void OnTimerFinished()
    {
        Debug.Log("Light timer finished!");

        // Turn off the light
        if (lightInteraction != null)
        {
            lightInteraction.light.enabled = false;
            lightInteraction.ResetLight();
        }

        // Hide the countdown text
        if (uiTimerText != null)
        {
            uiTimerText.enabled = false;
        }
    }
}




