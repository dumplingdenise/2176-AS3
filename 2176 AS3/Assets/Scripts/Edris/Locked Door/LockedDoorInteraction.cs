using System.Collections;
using UnityEngine;

public class LockedDoorInteraction : MonoBehaviour
{
    // How far the door rotates when opening (90° by default)
    public float openAngle = 90f;

    // How fast the door opens/closes
    public float openSpeed = 2f;

    // Tracks whether the door is currently open
    private bool isOpen = false;

    // Stores door rotation values for closed and opened states
    private Quaternion _closedRotation;
    private Quaternion _openRotationForward;
    private Quaternion _openRotationBackward;

    public Transform player;

    public GameObject interactionUI;

    public GameObject lockedUI;

    private bool playerInRange = false;

    void Start()
    {
        // Save the original rotation so player return the door to its closed state
        _closedRotation = transform.rotation;

        /* Calculate two possible open rotations (forward/backward)
           This allows the door to open AWAY from the player */
        _openRotationForward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        _openRotationBackward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -openAngle, 0));

        // Hide UI at start and warn if not assigned
        if (interactionUI != null)
            interactionUI.SetActive(false);
        else
            Debug.LogWarning("LockedDoorInteraction: interactionUI not assigned!");

        if (lockedUI != null)
            lockedUI.SetActive(false);
        else
            Debug.LogWarning("LockedDoorInteraction: lockedUI not assigned!");

        // Warn if the player reference wasn't set in the Inspector
        if (player == null)
            Debug.LogWarning("LockedDoorInteraction: player reference missing!");
    }

    void Update()
    {
        // If player is not near the door, stop checking inputs
        if (!playerInRange) return;

        // Player HAS the key
        if (KeyPickup.playerHasKey)
        {
            // Show "Left click to open" UI
            if (interactionUI != null) interactionUI.SetActive(true);

            // Hide "Door locked" UI
            if (lockedUI != null) lockedUI.SetActive(false);

            // If player clicks left click mouse, start opening/closing animation
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(ToggleDoor());
            }
        }
        else // Player DOES NOT have the key
        {
            if (interactionUI != null) interactionUI.SetActive(false);
            if (lockedUI != null) lockedUI.SetActive(true);

            // Play locked sound when clicking
            if (Input.GetMouseButtonDown(0))
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.PlaySound("DoorLocked");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Player enters trigger area
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Immediately show correct UI depending on key ownership
            if (KeyPickup.playerHasKey)
            {
                if (interactionUI != null) interactionUI.SetActive(true);
                if (lockedUI != null) lockedUI.SetActive(false);
            }
            else
            {
                if (interactionUI != null) interactionUI.SetActive(false);
                if (lockedUI != null) lockedUI.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Player leaves trigger area
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // Hide all UI when player walks away
            if (interactionUI != null) interactionUI.SetActive(false);
            if (lockedUI != null) lockedUI.SetActive(false);
        }
    }

    IEnumerator ToggleDoor()
    {

        if (player == null) yield break;

        isOpen = !isOpen;

        // Play open/close sound (only if scene isn't transitioning)
        if (!GameManager.isSceneTransitioning && AudioManager.instance != null)
            AudioManager.instance.PlaySound(isOpen ? "DoorOpen" : "DoorClose");

        Quaternion targetRot = _closedRotation;

        if (isOpen)
        {
            // Vector from door to player
            Vector3 toPlayer = (player.position - transform.position).normalized;

            // Dot product tells whether player is in front or behind the door
            float dot = Vector3.Dot(transform.forward, toPlayer);

            // If player is in front, open away from them
            targetRot = (dot > 0) ? _openRotationBackward : _openRotationForward;
        }

        // Smoothly rotate door over multiple frames
        while (Quaternion.Angle(transform.rotation, targetRot) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                openSpeed * 100f * Time.deltaTime   // Multiply for adjustable speed
            );
            yield return null; 
        }

        transform.rotation = targetRot;
    }
}


