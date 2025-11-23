using UnityEngine;
using TMPro;

public class PushBox3DTextUI : MonoBehaviour
{
    [Header("Text Prompt")]
    public TextMeshPro promptText;              
    [Header("Push logic reference")]
    public PushInteraction_Shumin pushInteraction;

    void Start()
    {
        if (promptText != null)
            promptText.enabled = false;
    }

    void Update()
    {
        if (promptText == null || pushInteraction == null) return;

        // If box is unlocked (currently pushable), hide text
        if (pushInteraction.isUnlocked && promptText.enabled)
        {
            promptText.enabled = false;
        }
        // If player is near AND box is locked, show text
        else if (!pushInteraction.isUnlocked && pushInteraction.isPlayerNear && !promptText.enabled)
        {
            promptText.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (promptText != null)
            promptText.enabled = false;
    }
}
