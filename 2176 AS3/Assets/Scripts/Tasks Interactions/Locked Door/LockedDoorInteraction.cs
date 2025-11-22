using System.Collections;
using UnityEngine;

public class LockedDoorInteraction : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen = false;
    private Quaternion _closedRotation;
    private Quaternion _openRotationForward;
    private Quaternion _openRotationBackward;

    public Transform player;
    public GameObject interactionUI;        // "Left click to open"
    public GameObject lockedUI;             // "Door is locked"

    private bool playerInRange = false;

    void Start()
    {
        _closedRotation = transform.rotation;

        _openRotationForward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        _openRotationBackward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -openAngle, 0));

        interactionUI.SetActive(false);
        lockedUI.SetActive(false);
    }

    void Update()
    {
        if (playerInRange)
        {
            if (KeyPickup.playerHasKey)
            {
                // Show interaction UI, hide locked UI
                interactionUI.SetActive(true);
                lockedUI.SetActive(false);

                if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(ToggleDoor());
                }
            }
            else
            {
                // Show locked UI, hide interaction UI
                interactionUI.SetActive(false);
                lockedUI.SetActive(true);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Show correct UI immediately
            if (KeyPickup.playerHasKey)
            {
                interactionUI.SetActive(true);
                lockedUI.SetActive(false);
            }
            else
            {
                interactionUI.SetActive(false);
                lockedUI.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionUI.SetActive(false);
            lockedUI.SetActive(false);
        }
    }

    IEnumerator ToggleDoor()
    {
        isOpen = !isOpen;

        Quaternion targetRot = _closedRotation;

        if (isOpen)
        {
            Vector3 toPlayer = (player.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, toPlayer);

            targetRot = (dot > 0) ? _openRotationBackward : _openRotationForward;
        }

        while (Quaternion.Angle(transform.rotation, targetRot) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                openSpeed * 100f * Time.deltaTime
            );
            yield return null;
        }

        transform.rotation = targetRot;
    }
}
