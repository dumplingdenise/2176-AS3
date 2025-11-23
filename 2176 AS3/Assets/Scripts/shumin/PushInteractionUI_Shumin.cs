using UnityEngine;
using TMPro;

public class PushBox3DTextUI : MonoBehaviour
{
    [Header("3D Text on the box")]
    public TextMeshPro promptText;

    [Header("Push logic reference")]
    public PushInteraction_Shumin pushInteraction;

    private void Start()
    {
        if (promptText != null)
            promptText.enabled = false;
    }

    private void Update()
    {
        // If box has been unlocked, make sure text is off forever
        if (pushInteraction != null && pushInteraction.HasUnlocked)
        {
            if (promptText != null && promptText.enabled)
                promptText.enabled = false;

            // enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (promptText == null || pushInteraction == null) return;

        // Only show if box has NOT been unlocked yet
        if (!pushInteraction.HasUnlocked)
            promptText.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (promptText == null) return;

        promptText.enabled = false;
    }
}
