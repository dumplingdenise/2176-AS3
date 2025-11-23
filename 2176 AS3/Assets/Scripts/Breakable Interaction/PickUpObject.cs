using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public Transform holdPoint;     // Assign from Inspector (player's HoldPoint)
    public Transform player;        // Assign player's transform here
    public float throwForce = 8f;

    private bool playerInRange = false;
    private bool isHeld = false;

    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isHeld)
            {
                Pickup();
            }
            else
            {
                Throw();
            }
        }
    }

    void Pickup()
    {
        isHeld = true;

        rb.isKinematic = true;
        col.isTrigger = true;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    void Throw()
    {
        isHeld = false;

        transform.SetParent(null);

        rb.isKinematic = false;
        col.isTrigger = false;

        // Throw in the player's facing direction
        rb.linearVelocity = player.forward * throwForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.LogError("Player in range");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
