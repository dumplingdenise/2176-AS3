using UnityEngine;
using UnityEngine.UI;

public class ExitTrigger : MonoBehaviour
{
    public bool doorUnlocked = false;
    public GameObject victoryPanel;
    public Image fadePanel;
    public ParticleSystem confetti;
    public float fadeSpeed = 1f; // the fade to black is set to be adjustable in Inspector

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && doorUnlocked && !triggered)
        {
            triggered = true;
            StartCoroutine(VictorySequence());
        }
    }

    private System.Collections.IEnumerator VictorySequence()
    {
        // Play confetti system
        confetti.Play();

        // Fade to black
        Color c = fadePanel.color;
        while (c.a < 1)
        {
            c.a += Time.deltaTime * fadeSpeed;
            fadePanel.color = c;
            yield return null;
        }

        // Trigger to show the UI
        victoryPanel.SetActive(true);

        // Stop the gameplay
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }
}
















