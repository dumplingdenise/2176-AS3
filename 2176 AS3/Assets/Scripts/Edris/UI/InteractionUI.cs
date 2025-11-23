using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI instance;

    public TextMeshProUGUI interactionText;

    void Awake()
    {
        instance = this;
        interactionText.text = "";
    }

    public void ShowMessage(string message)     // Showcasing to the player what action is needed.
    {
        interactionText.text = message;
    }

    public void ClearMessage()                  // After completing the required task.
    {
        interactionText.text = "";
    }
}

