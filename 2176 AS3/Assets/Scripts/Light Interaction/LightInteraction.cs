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

    [Header("Cooldown swtting")]
    public float cooldownDuration = 3f;

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

        Debug.Log("ToggleLight(): activated before press = " + activated);

        // AUDIO
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound("LightSwitchOn");
        }

        light.enabled = true;
        activated = true;
        lightText.enabled = false;


        // INTERACTION TRACKING
        if (gameManager != null) gameManager.TryCompleteTask(this.gameObject);
        if (uiManager != null && isTimed)
        {
            uiManager.StartLightTimer(lightDuration, this);
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

    public void ResetLight()
    {
        activated = false;
        light.enabled = false;
        lightText.enabled = false;

        Debug.Log("ResetLight(): activated = " + activated);
    }

    public bool CanInteract => !activated && lightText.enabled;
}
