using UnityEngine;

public class PushInteraction_Shumin : MonoBehaviour
{
    public Rigidbody rb;    // drag your box here
    private bool isplayerNear = false;

    private void Start()
    {
        rb.isKinematic = true;   // box starts locked/frozen
    }

    private void Update()
    {
        // Only unlock when player is near AND presses E
        if (isplayerNear && Input.GetKeyDown(KeyCode.E))
        {
            rb.isKinematic = false;   // box becomes pushable
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isplayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isplayerNear = false;
            rb.isKinematic = true; // box locked when player leaves
        }
    }
}
