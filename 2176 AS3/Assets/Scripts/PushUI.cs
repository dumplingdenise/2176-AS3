using UnityEngine;

public class PushUI : MonoBehaviour
{
    public GameObject interactionUI; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);   // hide at start
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && interactionUI != null)
        {
            interactionUI.SetActive(true);    // show text when near
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && interactionUI != null)
        {
            interactionUI.SetActive(false);   // hide text when leaving
        }
    }
}
