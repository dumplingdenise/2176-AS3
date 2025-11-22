using System.Collections;
using UnityEngine;

public class LockedDoorInteraction : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;

    public Transform player;

    private bool isOpen = false;
    private bool playerInRange = false;

    private Quaternion closedRotation;
    private Quaternion openRotationForward;
    private Quaternion openRotationBackward;

    void Start()
    {
        closedRotation = transform.rotation;

        openRotationForward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
        openRotationBackward = Quaternion.Euler(transform.eulerAngles + new Vector3(0, -openAngle, 0));
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }
    }

    void TryOpenDoor()
    {
        if (!PlayerInventory.hasKey)
        {
            InteractionUI.instance.ShowMessage("Door Locked. Find a key.");
            return;
        }

        StartCoroutine(ToggleDoor());
    }

    IEnumerator ToggleDoor()
    {
        isOpen = !isOpen;

        Quaternion targetRot = closedRotation;

        if (isOpen)
        {
            Vector3 toPlayer = (player.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, toPlayer);

            targetRot = (dot > 0) ? openRotationBackward : openRotationForward;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (!PlayerInventory.hasKey)
                InteractionUI.instance.ShowMessage("Door Locked");
            else
                InteractionUI.instance.ShowMessage("Press E to open door");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            InteractionUI.instance.ClearMessage();
        }
    }
}
