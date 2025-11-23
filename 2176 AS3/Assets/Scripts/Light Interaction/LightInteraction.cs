using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LightInteraction : MonoBehaviour
{
    public TextMeshPro lightText;
    public Light light;

    private bool activated = false;

    [Header("Manager References")]
    public UIManager uiManager;
    public GameManager gameManager;

    [Header("Settings")]
    public bool isTimed = true;
    public float lightDuration = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (lightText == null)
        {
            lightText = GetComponentInChildren<TextMeshPro>();
        }

        if (light == null)
        {
            light = GetComponentInChildren<Light>();
        }

        lightText.enabled = false;
        light.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void ShowText()
    {
        if (!activated)
            lightText.enabled = true;
    }

    public void HideText()
    {
        lightText.enabled = false;
    }

    public void ToggleLight()
    {
        if (activated) return;

        // AUDIO
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound("LightSwitchOn");
        }

        light.enabled = true;
        activated = true;
        lightText.enabled = false;

        // tell game manager and ui manager
        if (gameManager != null) gameManager.CompleteTask();
        if (uiManager != null && isTimed)
        {
            uiManager.StartLightTimer(lightDuration, this); // 'this' passes a reference to this script
        }
    }

    // public function for UIManager to call when the timer is done
    public void TurnOffLight()
    {
        if (light != null)
        {
            light.enabled = false;
        }
    }

    public bool CanInteract => !activated && lightText.enabled;
}
