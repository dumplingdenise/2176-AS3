using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LightInteraction : MonoBehaviour
{
    public TextMeshPro lightText;
    public Light light;

    private bool activated = false;

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

        light.enabled = true;
        activated = true;
        lightText.enabled = false;
    }

    public bool CanInteract => !activated && lightText.enabled;
}
