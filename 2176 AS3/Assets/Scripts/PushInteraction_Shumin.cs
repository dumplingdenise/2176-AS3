using UnityEngine;

public class PushInteraction_Shumin : MonoBehaviour
{
    public Rigidbody rb;
    private bool isplayerNear = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.isKinematic = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isplayerNear && Input.GetKeyDown(KeyCode.E))
        {
            //unlock when near and press E
            rb.isKinematic = false;
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

            // lock the pushable if player leaves the zone
            rb.isKinematic = true;
        }
    }
}
