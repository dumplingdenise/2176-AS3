using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;        // How far the door swings
    public float openSpeed = 2f;         // Speed of rotation

    private bool isOpen = false;         // Tracks if the door is currently open
    private bool isMoving = false;       // Prevents starting multiple coroutines

    private Quaternion _closedRotation;         // Original rotation of the door
    private Quaternion _openRotationForward;    // Rotation for opening forward
    private Quaternion _openRotationBackward;   // Rotation for opening backward

    [Header("Player Reference")]
    public Transform player; // Assign the player GameObject in the inspector

    [Header("Interaction UI(s)")]
    public GameObject[] interactionUIs; // Array for front and back UI texts

    private bool canOpen = false; // True if player is inside the trigger

    public GameManager gameManager;

    private NavMeshObstacle navMeshObstacle;

    void Start()
    {
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.enabled = true; // Start with the door closed and blocking the path

        // WARNINGS
        if (player == null) Debug.LogWarning("Player reference is missing!");
        if (interactionUIs == null || interactionUIs.Length < 2)
            Debug.LogWarning("Please assign front and back UI texts to interactionUIs array!");

        // Store the door's original rotation
        _closedRotation = transform.rotation;

        // Calculate the target rotations for opening in both directions
        _openRotationForward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        _openRotationBackward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -openAngle, 0));

        HideAllUI(); // Hide UI at start
    }

    void Update()
    {
        if (!canOpen || player == null) return; // safety check

        // Open the door when player is inside trigger and left-clicks
        if (Input.GetMouseButtonDown(0))
        {
            HideAllUI();                  // Hide UI while door is moving
            StartCoroutine(ToggleDoor()); // Start smooth rotation
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && player != null)
        {
            canOpen = true;

            if (interactionUIs != null && interactionUIs.Length >= 2)
            {
                // Determine which side of the door the player is on
                Vector3 toPlayer = (player.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, toPlayer);

                if (dot > 0)
                {
                    if (interactionUIs[1] != null) interactionUIs[1].SetActive(true); // Show back text
                    if (interactionUIs[0] != null) interactionUIs[0].SetActive(false);
                }
                else
                {
                    if (interactionUIs[0] != null) interactionUIs[0].SetActive(true); // Show front text
                    if (interactionUIs[1] != null) interactionUIs[1].SetActive(false);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canOpen = false;
            HideAllUI(); // Hide both UIs when leaving trigger
        }
    }

    // Helper method to hide all interaction UI elements
    private void HideAllUI()
    {
        if (interactionUIs == null) return;

        foreach (var ui in interactionUIs)
        {
            if (ui != null)
                ui.SetActive(false);
        }
    }

    // Coroutine to smoothly open/close the door
    IEnumerator ToggleDoor()
    {
        if (isMoving || player == null) yield break; // safety check

        isMoving = true;
        isOpen = !isOpen; // Toggle door state

        // If the door is now open, disable the obstacle. If it's closed, enable it.
        navMeshObstacle.enabled = !isOpen;

        Quaternion targetRot = _closedRotation;

        if (isOpen)
        {
            // Choose the rotation direction based on player position
            Vector3 toPlayer = (player.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, toPlayer);
            targetRot = (dot > 0) ? _openRotationBackward : _openRotationForward;
        }

        // AUDIO (optional safety check)
        if (!GameManager.isSceneTransitioning && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(isOpen ? "DoorOpen" : "DoorClose");
        }

        // INTERACTION TRACKING - only try to complete task when door is opening
        if (isOpen && gameManager != null)
        {
            gameManager.TryCompleteTask(this.gameObject);
        }

        // Smooth rotation over time
        while (Quaternion.Angle(transform.rotation, targetRot) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                openSpeed * 100f * Time.deltaTime
            );
            yield return null;
        }

        transform.rotation = targetRot; // Ensure exact final rotation
        isMoving = false;
    }
}





